using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Validators.Avaliacoes;
using Xunit;

namespace LibraryDev.Tests.Validators;

public class AvaliacaoValidatorTests
{
    private static CriarAvaliacaoCommand CriarCommandValido() => new()
    {
        Nota = 4,
        Descricao = "Bom livro",
        IdUsuario = 1,
        IdLivro = 1,
        DataInicioLeitura = new DateTime(2025, 1, 1),
        DataFimLeitura = new DateTime(2025, 1, 15)
    };

    // ── ValidarCriar ───────────────────────────────────────────

    [Fact]
    public void ValidarCriar_DadosValidos_RetornaValido()
    {
        var (valido, _) = AvaliacaoValidator.ValidarCriar(CriarCommandValido());
        Assert.True(valido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public void ValidarCriar_NotaForaDoRange_RetornaInvalido(int nota)
    {
        var cmd = CriarCommandValido();
        cmd.Nota = nota;
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Nota deve ser entre 1 e 5.", mensagem);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarCriar_IdUsuarioInvalido_RetornaInvalido(int idUsuario)
    {
        var cmd = CriarCommandValido();
        cmd.IdUsuario = idUsuario;
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("IdUsuario inválido.", mensagem);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarCriar_IdLivroInvalido_RetornaInvalido(int idLivro)
    {
        var cmd = CriarCommandValido();
        cmd.IdLivro = idLivro;
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("IdLivro inválido.", mensagem);
    }

    [Fact]
    public void ValidarCriar_DataInicioDefault_RetornaInvalido()
    {
        var cmd = CriarCommandValido();
        cmd.DataInicioLeitura = default;
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Data de início de leitura é obrigatória.", mensagem);
    }

    [Fact]
    public void ValidarCriar_DataFimDefault_RetornaInvalido()
    {
        var cmd = CriarCommandValido();
        cmd.DataFimLeitura = default;
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Data de fim de leitura é obrigatória.", mensagem);
    }

    [Fact]
    public void ValidarCriar_DataInicioMaiorQueDataFim_RetornaInvalido()
    {
        var cmd = CriarCommandValido();
        cmd.DataInicioLeitura = new DateTime(2025, 2, 1);
        cmd.DataFimLeitura = new DateTime(2025, 1, 1);
        var (valido, mensagem) = AvaliacaoValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Data de início de leitura não pode ser maior que a data de fim.", mensagem);
    }

    // ── ValidarAtualizar ───────────────────────────────────────

    [Fact]
    public void ValidarAtualizar_IdInvalido_RetornaInvalido()
    {
        var cmd = new AtualizarAvaliacaoCommand
        {
            Id = 0,
            Nota = 4,
            IdUsuario = 1,
            IdLivro = 1,
            DataInicioLeitura = new DateTime(2025, 1, 1),
            DataFimLeitura = new DateTime(2025, 1, 15)
        };
        var (valido, mensagem) = AvaliacaoValidator.ValidarAtualizar(cmd);
        Assert.False(valido);
        Assert.Equal("Id inválido.", mensagem);
    }

    [Fact]
    public void ValidarAtualizar_DadosValidos_RetornaValido()
    {
        var cmd = new AtualizarAvaliacaoCommand
        {
            Id = 1,
            Nota = 4,
            IdUsuario = 1,
            IdLivro = 1,
            DataInicioLeitura = new DateTime(2025, 1, 1),
            DataFimLeitura = new DateTime(2025, 1, 15)
        };
        var (valido, _) = AvaliacaoValidator.ValidarAtualizar(cmd);
        Assert.True(valido);
    }
}
