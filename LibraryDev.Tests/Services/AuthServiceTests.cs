using LibraryDev.Application.Commands.Auth;
using LibraryDev.Application.Services;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Usuarios;
using LibraryDev.Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LibraryDev.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioQueryRepository> _queryRepoMock = new();
    private readonly Mock<IUsuarioCommandRepository> _commandRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly IConfiguration _configuration;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var configData = new Dictionary<string, string?>
        {
            ["JwtSettings:Secret"] = "UmaChaveSecretaComMaisDe32CaracteresParaTeste!!",
            ["JwtSettings:Issuer"] = "LibraryDev",
            ["JwtSettings:Audience"] = "LibraryDev",
            ["JwtSettings:AccessTokenExpirationMinutes"] = "60",
            ["JwtSettings:RefreshTokenExpirationDays"] = "7"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _sut = new AuthService(
            _queryRepoMock.Object,
            _commandRepoMock.Object,
            _emailServiceMock.Object,
            _configuration);
    }

    // ── Login ──────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_CredenciaisValidas_RetornaToken()
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123");
        var usuario = new Usuario { Id = 1, Email = "user@email.com", Nome = "User", Senha = senhaHash };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorEmailAsync("user@email.com"))
            .ReturnsAsync(usuario);

        var (sucesso, _, resultado) = await _sut.LoginAsync(new LoginCommand("user@email.com", "Senha@123"));

        Assert.True(sucesso);
        Assert.NotNull(resultado);
        Assert.False(string.IsNullOrEmpty(resultado!.AccessToken));
        Assert.False(string.IsNullOrEmpty(resultado.RefreshToken));
        _commandRepoMock.Verify(r => r.AtualizarRefreshTokenAsync(1, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_EmailInexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioPorEmailAsync("nao@existe.com"))
            .ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem, _) = await _sut.LoginAsync(new LoginCommand("nao@existe.com", "Senha@123"));

        Assert.False(sucesso);
        Assert.Equal("E-mail ou senha inválidos.", mensagem);
    }

    [Fact]
    public async Task LoginAsync_SenhaIncorreta_RetornaFalha()
    {
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("Senha@123");
        var usuario = new Usuario { Id = 1, Email = "user@email.com", Nome = "User", Senha = senhaHash };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorEmailAsync("user@email.com"))
            .ReturnsAsync(usuario);

        var (sucesso, mensagem, _) = await _sut.LoginAsync(new LoginCommand("user@email.com", "SenhaErrada!1"));

        Assert.False(sucesso);
        Assert.Equal("E-mail ou senha inválidos.", mensagem);
    }

    [Fact]
    public async Task LoginAsync_EmailVazio_RetornaFalhaValidacao()
    {
        var (sucesso, mensagem, _) = await _sut.LoginAsync(new LoginCommand("", "Senha@123"));

        Assert.False(sucesso);
        Assert.Equal("E-mail é obrigatório.", mensagem);
    }

    [Fact]
    public async Task LoginAsync_SenhaVazia_RetornaFalhaValidacao()
    {
        var (sucesso, mensagem, _) = await _sut.LoginAsync(new LoginCommand("user@email.com", ""));

        Assert.False(sucesso);
        Assert.Equal("Senha é obrigatória.", mensagem);
    }

    // ── RefreshToken ───────────────────────────────────────────

    [Fact]
    public async Task RefreshTokenAsync_TokenValido_RetornaNovoToken()
    {
        var usuario = new Usuario
        {
            Id = 1, Email = "user@email.com", Nome = "User", Senha = "hash",
            RefreshToken = "valid-refresh-token",
            RefreshTokenExpiracao = DateTime.UtcNow.AddDays(1)
        };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorRefreshTokenAsync("valid-refresh-token"))
            .ReturnsAsync(usuario);

        var (sucesso, _, resultado) = await _sut.RefreshTokenAsync(new RefreshTokenCommand("valid-refresh-token"));

        Assert.True(sucesso);
        Assert.NotNull(resultado);
        Assert.False(string.IsNullOrEmpty(resultado!.AccessToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_TokenExpirado_RetornaFalha()
    {
        var usuario = new Usuario
        {
            Id = 1, Email = "user@email.com", Nome = "User", Senha = "hash",
            RefreshToken = "expired-token",
            RefreshTokenExpiracao = DateTime.UtcNow.AddDays(-1)
        };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorRefreshTokenAsync("expired-token"))
            .ReturnsAsync(usuario);

        var (sucesso, mensagem, _) = await _sut.RefreshTokenAsync(new RefreshTokenCommand("expired-token"));

        Assert.False(sucesso);
        Assert.Equal("Refresh token inválido ou expirado.", mensagem);
    }

    [Fact]
    public async Task RefreshTokenAsync_TokenInexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioPorRefreshTokenAsync("inexistente"))
            .ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem, _) = await _sut.RefreshTokenAsync(new RefreshTokenCommand("inexistente"));

        Assert.False(sucesso);
        Assert.Equal("Refresh token inválido ou expirado.", mensagem);
    }

    [Fact]
    public async Task RefreshTokenAsync_TokenVazio_RetornaFalha()
    {
        var (sucesso, mensagem, _) = await _sut.RefreshTokenAsync(new RefreshTokenCommand(""));

        Assert.False(sucesso);
        Assert.Equal("Refresh token é obrigatório.", mensagem);
    }

    // ── RecuperarSenha ─────────────────────────────────────────

    [Fact]
    public async Task RecuperarSenhaAsync_EmailExistente_EnviaEmail()
    {
        var usuario = new Usuario { Id = 1, Email = "user@email.com", Nome = "User", Senha = "hash" };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorEmailAsync("user@email.com"))
            .ReturnsAsync(usuario);

        var (sucesso, _) = await _sut.RecuperarSenhaAsync(new RecuperarSenhaCommand("user@email.com"));

        Assert.True(sucesso);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperacaoSenhaAsync("user@email.com", "User", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RecuperarSenhaAsync_EmailInexistente_RetornaSucessoSemRevelar()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioPorEmailAsync("nao@existe.com"))
            .ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem) = await _sut.RecuperarSenhaAsync(new RecuperarSenhaCommand("nao@existe.com"));

        Assert.True(sucesso);
        Assert.Contains("Se o e-mail estiver cadastrado", mensagem);
        _emailServiceMock.Verify(e => e.EnviarEmailRecuperacaoSenhaAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RecuperarSenhaAsync_EmailVazio_RetornaSucesso()
    {
        var (sucesso, _) = await _sut.RecuperarSenhaAsync(new RecuperarSenhaCommand(""));

        Assert.True(sucesso);
    }

    // ── RedefinirSenha ─────────────────────────────────────────

    [Fact]
    public async Task RedefinirSenhaAsync_TokenValido_AtualizaSenha()
    {
        var usuario = new Usuario
        {
            Id = 1, Email = "user@email.com", Nome = "User", Senha = "old-hash",
            TokenRecuperacaoSenha = "valid-token",
            TokenRecuperacaoExpiracao = DateTime.UtcNow.AddMinutes(10)
        };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorTokenRecuperacaoAsync("valid-token"))
            .ReturnsAsync(usuario);

        var (sucesso, mensagem) = await _sut.RedefinirSenhaAsync(new RedefinirSenhaCommand("valid-token", "NovaSenha@1"));

        Assert.True(sucesso);
        Assert.Equal("Senha redefinida com sucesso.", mensagem);
        _commandRepoMock.Verify(r => r.AtualizarSenhaAsync(1, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RedefinirSenhaAsync_TokenExpirado_RetornaFalha()
    {
        var usuario = new Usuario
        {
            Id = 1, Email = "user@email.com", Nome = "User", Senha = "hash",
            TokenRecuperacaoSenha = "expired-token",
            TokenRecuperacaoExpiracao = DateTime.UtcNow.AddMinutes(-10)
        };

        _queryRepoMock.Setup(r => r.ObterUsuarioPorTokenRecuperacaoAsync("expired-token"))
            .ReturnsAsync(usuario);

        var (sucesso, mensagem) = await _sut.RedefinirSenhaAsync(new RedefinirSenhaCommand("expired-token", "NovaSenha@1"));

        Assert.False(sucesso);
        Assert.Equal("Token inválido ou expirado.", mensagem);
    }

    [Fact]
    public async Task RedefinirSenhaAsync_TokenInexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioPorTokenRecuperacaoAsync("nao-existe"))
            .ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem) = await _sut.RedefinirSenhaAsync(new RedefinirSenhaCommand("nao-existe", "NovaSenha@1"));

        Assert.False(sucesso);
        Assert.Equal("Token inválido ou expirado.", mensagem);
    }

    [Fact]
    public async Task RedefinirSenhaAsync_SenhaFraca_RetornaFalhaValidacao()
    {
        var (sucesso, _) = await _sut.RedefinirSenhaAsync(new RedefinirSenhaCommand("valid-token", "abc"));

        Assert.False(sucesso);
    }
}
