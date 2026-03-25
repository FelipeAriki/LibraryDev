using LibraryDev.Application.Commands.Auth;
using LibraryDev.Application.Validators.Auth;
using Xunit;

namespace LibraryDev.Tests.Validators;

public class AuthValidatorTests
{
    // ── ValidarLogin ───────────────────────────────────────────

    [Fact]
    public void ValidarLogin_EmailVazio_RetornaInvalido()
    {
        var command = new LoginCommand("", "Senha@123");
        var (valido, mensagem) = AuthValidator.ValidarLogin(command);
        Assert.False(valido);
        Assert.Equal("E-mail é obrigatório.", mensagem);
    }

    [Fact]
    public void ValidarLogin_SenhaVazia_RetornaInvalido()
    {
        var command = new LoginCommand("user@email.com", "");
        var (valido, mensagem) = AuthValidator.ValidarLogin(command);
        Assert.False(valido);
        Assert.Equal("Senha é obrigatória.", mensagem);
    }

    [Fact]
    public void ValidarLogin_DadosValidos_RetornaValido()
    {
        var command = new LoginCommand("user@email.com", "Senha@123");
        var (valido, _) = AuthValidator.ValidarLogin(command);
        Assert.True(valido);
    }

    // ── ValidarSenha ───────────────────────────────────────────

    [Theory]
    [InlineData(null, "Senha é obrigatória.")]
    [InlineData("", "Senha é obrigatória.")]
    [InlineData("   ", "Senha é obrigatória.")]
    public void ValidarSenha_VaziaOuNula_RetornaInvalido(string? senha, string mensagemEsperada)
    {
        var (valido, mensagem) = AuthValidator.ValidarSenha(senha);
        Assert.False(valido);
        Assert.Equal(mensagemEsperada, mensagem);
    }

    [Fact]
    public void ValidarSenha_MenosDe8Caracteres_RetornaInvalido()
    {
        var (valido, mensagem) = AuthValidator.ValidarSenha("Ab1!xyz");
        Assert.False(valido);
        Assert.Equal("Senha deve ter no mínimo 8 caracteres.", mensagem);
    }

    [Fact]
    public void ValidarSenha_SemMaiuscula_RetornaInvalido()
    {
        var (valido, mensagem) = AuthValidator.ValidarSenha("abcdefg1!");
        Assert.False(valido);
        Assert.Equal("Senha deve conter pelo menos uma letra maiúscula.", mensagem);
    }

    [Fact]
    public void ValidarSenha_SemMinuscula_RetornaInvalido()
    {
        var (valido, mensagem) = AuthValidator.ValidarSenha("ABCDEFG1!");
        Assert.False(valido);
        Assert.Equal("Senha deve conter pelo menos uma letra minúscula.", mensagem);
    }

    [Fact]
    public void ValidarSenha_SemDigito_RetornaInvalido()
    {
        var (valido, mensagem) = AuthValidator.ValidarSenha("Abcdefgh!");
        Assert.False(valido);
        Assert.Equal("Senha deve conter pelo menos um número.", mensagem);
    }

    [Fact]
    public void ValidarSenha_SemEspecial_RetornaInvalido()
    {
        var (valido, mensagem) = AuthValidator.ValidarSenha("Abcdefg1");
        Assert.False(valido);
        Assert.Equal("Senha deve conter pelo menos um caractere especial.", mensagem);
    }

    [Fact]
    public void ValidarSenha_Valida_RetornaValido()
    {
        var (valido, _) = AuthValidator.ValidarSenha("Abcdefg1!");
        Assert.True(valido);
    }

    // ── ValidarRedefinirSenha ──────────────────────────────────

    [Fact]
    public void ValidarRedefinirSenha_TokenVazio_RetornaInvalido()
    {
        var command = new RedefinirSenhaCommand("", "Abcdefg1!");
        var (valido, mensagem) = AuthValidator.ValidarRedefinirSenha(command);
        Assert.False(valido);
        Assert.Equal("Token é obrigatório.", mensagem);
    }

    [Fact]
    public void ValidarRedefinirSenha_SenhaFraca_RetornaInvalido()
    {
        var command = new RedefinirSenhaCommand("valid-token", "abc");
        var (valido, _) = AuthValidator.ValidarRedefinirSenha(command);
        Assert.False(valido);
    }

    [Fact]
    public void ValidarRedefinirSenha_Valido_RetornaValido()
    {
        var command = new RedefinirSenhaCommand("valid-token", "Abcdefg1!");
        var (valido, _) = AuthValidator.ValidarRedefinirSenha(command);
        Assert.True(valido);
    }
}
