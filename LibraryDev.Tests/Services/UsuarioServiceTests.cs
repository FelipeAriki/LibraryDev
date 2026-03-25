using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Queries.Usuarios;
using LibraryDev.Application.Services;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Usuarios;
using Moq;
using Xunit;

namespace LibraryDev.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioCommandRepository> _commandRepoMock = new();
    private readonly Mock<IUsuarioQueryRepository> _queryRepoMock = new();
    private readonly UsuarioService _sut;

    public UsuarioServiceTests()
    {
        _sut = new UsuarioService(_commandRepoMock.Object, _queryRepoMock.Object);
    }

    // ── ObterUsuarios ──────────────────────────────────────────

    [Fact]
    public async Task ObterUsuariosAsync_RetornaListaComTotalAvaliacoes()
    {
        var usuarios = new List<UsuarioResumo>
        {
            new(1, "Felipe", "felipe@email.com", 3),
            new(2, "Ana", "ana@email.com", 0)
        };
        _queryRepoMock.Setup(r => r.ObterUsuariosAsync()).ReturnsAsync(usuarios);

        var resultado = await _sut.ObterUsuariosAsync();
        var lista = resultado.ToList();

        Assert.Equal(2, lista.Count);
        Assert.Equal(3, lista[0].TotalAvaliacoes);
        Assert.Equal(0, lista[1].TotalAvaliacoes);
    }

    // ── ObterUsuarioPorId ──────────────────────────────────────

    [Fact]
    public async Task ObterUsuarioPorIdAsync_Existente_RetornaViewModelComAvaliacoes()
    {
        var usuario = new Usuario
        {
            Id = 1, Nome = "Felipe", Email = "felipe@email.com",
            Avaliacoes =
            [
                new()
                {
                    Id = 1, Nota = 5, IdUsuario = 1, IdLivro = 1,
                    DataInicioLeitura = new DateTime(2025, 1, 1),
                    DataFimLeitura = new DateTime(2025, 1, 10),
                    DataCriacao = DateTime.UtcNow,
                    Livro = new Livro { Titulo = "Clean Code" }
                }
            ]
        };
        _queryRepoMock.Setup(r => r.ObterUsuarioComAvaliacoesPorIdAsync(1)).ReturnsAsync(usuario);

        var resultado = await _sut.ObterUsuarioPorIdAsync(new ObterUsuarioPorIdQuery(1));

        Assert.NotNull(resultado);
        Assert.Equal("Felipe", resultado!.Nome);
        Assert.Single(resultado.Avaliacoes);
        Assert.Equal("Clean Code", resultado.Avaliacoes[0].TituloLivro);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_Inexistente_RetornaNull()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioComAvaliacoesPorIdAsync(999)).ReturnsAsync((Usuario?)null);

        var resultado = await _sut.ObterUsuarioPorIdAsync(new ObterUsuarioPorIdQuery(999));

        Assert.Null(resultado);
    }

    // ── CriarUsuario ───────────────────────────────────────────

    [Fact]
    public async Task CriarUsuarioAsync_Valido_RetornaIdComSenhaHash()
    {
        var cmd = new CriarUsuarioCommand("Felipe", "felipe@email.com", "Senha@123");

        _queryRepoMock.Setup(r => r.EmailJaCadastradoAsync("felipe@email.com", null)).ReturnsAsync(false);
        _commandRepoMock.Setup(r => r.CriarUsuarioAsync(It.IsAny<Usuario>())).ReturnsAsync(10);

        var (sucesso, _, id) = await _sut.CriarUsuarioAsync(cmd);

        Assert.True(sucesso);
        Assert.Equal(10, id);
        _commandRepoMock.Verify(r => r.CriarUsuarioAsync(It.Is<Usuario>(u =>
            u.Senha != "Senha@123" && u.Senha.StartsWith("$2"))), Times.Once);
    }

    [Fact]
    public async Task CriarUsuarioAsync_EmailDuplicado_RetornaFalha()
    {
        var cmd = new CriarUsuarioCommand("Felipe", "felipe@email.com", "Senha@123");

        _queryRepoMock.Setup(r => r.EmailJaCadastradoAsync("felipe@email.com", null)).ReturnsAsync(true);

        var (sucesso, mensagem, _) = await _sut.CriarUsuarioAsync(cmd);

        Assert.False(sucesso);
        Assert.Contains("e-mail", mensagem, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CriarUsuarioAsync_NomeVazio_RetornaFalhaValidacao()
    {
        var cmd = new CriarUsuarioCommand("", "felipe@email.com", "Senha@123");

        var (sucesso, mensagem, _) = await _sut.CriarUsuarioAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Nome é obrigatório.", mensagem);
    }

    // ── AtualizarUsuario ───────────────────────────────────────

    [Fact]
    public async Task AtualizarUsuarioAsync_Existente_RetornaSucesso()
    {
        var cmd = new AtualizarUsuarioCommand(1, "Felipe Updated", "felipe@email.com");

        _queryRepoMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(new Usuario { Id = 1 });
        _queryRepoMock.Setup(r => r.EmailJaCadastradoAsync("felipe@email.com", 1)).ReturnsAsync(false);
        _commandRepoMock.Setup(r => r.AtualizarUsuarioAsync(It.IsAny<Usuario>())).ReturnsAsync(true);

        var (sucesso, _) = await _sut.AtualizarUsuarioAsync(cmd);

        Assert.True(sucesso);
    }

    [Fact]
    public async Task AtualizarUsuarioAsync_Inexistente_RetornaFalha()
    {
        var cmd = new AtualizarUsuarioCommand(999, "X", "x@email.com");

        _queryRepoMock.Setup(r => r.ObterUsuarioPorIdAsync(999)).ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem) = await _sut.AtualizarUsuarioAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Usuário não encontrado.", mensagem);
    }

    [Fact]
    public async Task AtualizarUsuarioAsync_EmailConflitoComOutro_RetornaFalha()
    {
        var cmd = new AtualizarUsuarioCommand(1, "Felipe", "outro@email.com");

        _queryRepoMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(new Usuario { Id = 1 });
        _queryRepoMock.Setup(r => r.EmailJaCadastradoAsync("outro@email.com", 1)).ReturnsAsync(true);

        var (sucesso, mensagem) = await _sut.AtualizarUsuarioAsync(cmd);

        Assert.False(sucesso);
        Assert.Contains("e-mail", mensagem, StringComparison.OrdinalIgnoreCase);
    }

    // ── DeletarUsuario ─────────────────────────────────────────

    [Fact]
    public async Task DeletarUsuarioAsync_Existente_RetornaSucesso()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(new Usuario { Id = 1 });
        _commandRepoMock.Setup(r => r.DeletarUsuarioAsync(1)).ReturnsAsync(true);

        var (sucesso, _) = await _sut.DeletarUsuarioAsync(1);

        Assert.True(sucesso);
    }

    [Fact]
    public async Task DeletarUsuarioAsync_Inexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterUsuarioPorIdAsync(999)).ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem) = await _sut.DeletarUsuarioAsync(999);

        Assert.False(sucesso);
        Assert.Equal("Usuário não encontrado.", mensagem);
    }
}
