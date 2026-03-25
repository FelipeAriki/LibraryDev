using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Validators.Livros;
using LibraryDev.Domain.Enums;
using Xunit;

namespace LibraryDev.Tests.Validators;

public class LivroValidatorTests
{
    private static CriarLivroCommand CriarCommandValido() => new()
    {
        Titulo = "Clean Code",
        ISBN = "978-0132350884",
        Autor = "Robert C. Martin",
        Editora = "Prentice Hall",
        Genero = GeneroLivroEnum.Ciencia,
        AnoDePublicacao = 2008,
        QuantidadePaginas = 464
    };

    // ── ValidarCriar ───────────────────────────────────────────

    [Fact]
    public void ValidarCriar_DadosValidos_RetornaValido()
    {
        var (valido, _) = LivroValidator.ValidarCriar(CriarCommandValido());
        Assert.True(valido);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarCriar_TituloVazio_RetornaInvalido(string? titulo)
    {
        var cmd = CriarCommandValido();
        cmd.Titulo = titulo!;
        var (valido, mensagem) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Título é obrigatório.", mensagem);
    }

    [Fact]
    public void ValidarCriar_TituloMaiorQue500_RetornaInvalido()
    {
        var cmd = CriarCommandValido();
        cmd.Titulo = new string('A', 501);
        var (valido, mensagem) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Título não pode ultrapassar 500 caracteres.", mensagem);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarCriar_ISBNVazio_RetornaInvalido(string? isbn)
    {
        var cmd = CriarCommandValido();
        cmd.ISBN = isbn!;
        var (valido, mensagem) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("ISBN é obrigatório.", mensagem);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarCriar_AutorVazio_RetornaInvalido(string? autor)
    {
        var cmd = CriarCommandValido();
        cmd.Autor = autor!;
        var (valido, mensagem) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Autor é obrigatório.", mensagem);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidarCriar_EditoraVazia_RetornaInvalido(string? editora)
    {
        var cmd = CriarCommandValido();
        cmd.Editora = editora!;
        var (valido, mensagem) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Editora é obrigatória.", mensagem);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarCriar_AnoPublicacaoMenorQue1_RetornaInvalido(int ano)
    {
        var cmd = CriarCommandValido();
        cmd.AnoDePublicacao = ano;
        var (valido, _) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
    }

    [Fact]
    public void ValidarCriar_AnoPublicacaoFuturo_RetornaInvalido()
    {
        var cmd = CriarCommandValido();
        cmd.AnoDePublicacao = DateTime.UtcNow.Year + 1;
        var (valido, _) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidarCriar_QuantidadePaginasInvalida_RetornaInvalido(int paginas)
    {
        var cmd = CriarCommandValido();
        cmd.QuantidadePaginas = paginas;
        var (valido, mensagem) = LivroValidator.ValidarCriar(cmd);
        Assert.False(valido);
        Assert.Equal("Quantidade de páginas deve ser maior que zero.", mensagem);
    }

    // ── ValidarAtualizar ───────────────────────────────────────

    [Fact]
    public void ValidarAtualizar_IdInvalido_RetornaInvalido()
    {
        var cmd = new AtualizarLivroCommand
        {
            Id = 0,
            Titulo = "Clean Code",
            ISBN = "978-0132350884",
            Autor = "Robert C. Martin",
            Editora = "Prentice Hall",
            AnoDePublicacao = 2008,
            QuantidadePaginas = 464
        };
        var (valido, mensagem) = LivroValidator.ValidarAtualizar(cmd);
        Assert.False(valido);
        Assert.Equal("Id inválido.", mensagem);
    }

    [Fact]
    public void ValidarAtualizar_DadosValidos_RetornaValido()
    {
        var cmd = new AtualizarLivroCommand
        {
            Id = 1,
            Titulo = "Clean Code",
            ISBN = "978-0132350884",
            Autor = "Robert C. Martin",
            Editora = "Prentice Hall",
            AnoDePublicacao = 2008,
            QuantidadePaginas = 464
        };
        var (valido, _) = LivroValidator.ValidarAtualizar(cmd);
        Assert.True(valido);
    }
}
