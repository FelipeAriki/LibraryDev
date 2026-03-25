using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Validators.Usuarios;
using Xunit;

namespace LibraryDev.Tests.Validators;

public class UsuarioValidatorTests
{
    // ── ValidarCriar ───────────────────────────────────────────

    [Fact]
    public void ValidarCriar_DadosValidos_RetornaValido()
    {
        var cmd = new CriarUsuarioCommand("Felipe", "felipe@email.com", "Senha@123");
        var (valido, _) = UsuarioValidator.ValidarCriar(cmd);
        Assert.True(valido);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarCriar_NomeVazio_RetornaInvalido(string? nome)
    {
        var cmd = new CriarUsuarioCommand(nome!, "felipe@email.com", "Senha@123");
        var (valido, mensagem) = UsuarioValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Nome é obrigatório.", mensagem);
    }

    [Fact]
    public void ValidarCriar_NomeMaiorQue200_RetornaInvalido()
    {
        var cmd = new CriarUsuarioCommand(new string('A', 201), "felipe@email.com", "Senha@123");
        var (valido, mensagem) = UsuarioValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Nome não pode ultrapassar 200 caracteres.", mensagem);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarCriar_EmailVazio_RetornaInvalido(string? email)
    {
        var cmd = new CriarUsuarioCommand("Felipe", email!, "Senha@123");
        var (valido, mensagem) = UsuarioValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("E-mail é obrigatório.", mensagem);
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("sem@dominio")]
    public void ValidarCriar_EmailInvalido_RetornaInvalido(string email)
    {
        var cmd = new CriarUsuarioCommand("Felipe", email, "Senha@123");
        var (valido, mensagem) = UsuarioValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("E-mail inválido.", mensagem);
    }

    [Fact]
    public void ValidarCriar_EmailMaiorQue320_RetornaInvalido()
    {
        var email = new string('a', 310) + "@email.com";
        var cmd = new CriarUsuarioCommand("Felipe", email, "Senha@123");
        var (valido, mensagem) = UsuarioValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("E-mail não pode ultrapassar 320 caracteres.", mensagem);
    }

    [Fact]
    public void ValidarCriar_SenhaFraca_RetornaInvalido()
    {
        var cmd = new CriarUsuarioCommand("Felipe", "felipe@email.com", "abc");
        var (valido, _) = UsuarioValidator.ValidarCriar(cmd);
        Assert.False(valido);
    }

    // ── ValidarAtualizar ───────────────────────────────────────

    [Fact]
    public void ValidarAtualizar_IdInvalido_RetornaInvalido()
    {
        var cmd = new AtualizarUsuarioCommand(0, "Felipe", "felipe@email.com");
        var (valido, mensagem) = UsuarioValidator.ValidarAtualizar(cmd);
        Assert.False(valido);
        Assert.Equal("Id inválido.", mensagem);
    }

    [Fact]
    public void ValidarAtualizar_DadosValidos_RetornaValido()
    {
        var cmd = new AtualizarUsuarioCommand(1, "Felipe", "felipe@email.com");
        var (valido, _) = UsuarioValidator.ValidarAtualizar(cmd);
        Assert.True(valido);
    }

    [Fact]
    public void ValidarAtualizar_NomeVazio_RetornaInvalido()
    {
        var cmd = new AtualizarUsuarioCommand(1, "", "felipe@email.com");
        var (valido, mensagem) = UsuarioValidator.ValidarAtualizar(cmd);
        Assert.False(valido);
        Assert.Equal("Nome é obrigatório.", mensagem);
    }

    [Fact]
    public void ValidarAtualizar_EmailInvalido_RetornaInvalido()
    {
        var cmd = new AtualizarUsuarioCommand(1, "Felipe", "invalido");
        var (valido, mensagem) = UsuarioValidator.ValidarAtualizar(cmd);
        Assert.False(valido);
        Assert.Equal("E-mail inválido.", mensagem);
    }
}
