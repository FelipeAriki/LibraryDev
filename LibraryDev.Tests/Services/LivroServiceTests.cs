using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Queries.Livros;
using LibraryDev.Application.Services;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Enums;
using LibraryDev.Domain.Interfaces.Livros;
using LibraryDev.Domain.Services;
using Moq;
using Xunit;

namespace LibraryDev.Tests.Services;

public class LivroServiceTests
{
    private readonly Mock<ILivroCommandRepository> _commandRepoMock = new();
    private readonly Mock<ILivroQueryRepository> _queryRepoMock = new();
    private readonly Mock<IOpenLibraryService> _openLibraryMock = new();
    private readonly LivroService _sut;

    public LivroServiceTests()
    {
        _sut = new LivroService(
            _commandRepoMock.Object,
            _queryRepoMock.Object,
            _openLibraryMock.Object);
    }

    private static Livro CriarLivroExemplo(int id = 1) => new()
    {
        Id = id,
        Titulo = "Clean Code",
        Descricao = "Guia de boas práticas",
        ISBN = "978-0132350884",
        Autor = "Robert C. Martin",
        Editora = "Prentice Hall",
        Genero = GeneroLivroEnum.Ciencia,
        AnoDePublicacao = 2008,
        QuantidadePaginas = 464,
        DataCriacao = DateTime.UtcNow,
        NotaMedia = 4.5m
    };

    private static CriarLivroCommand CriarCommandValido() => new()
    {
        Titulo = "Clean Code",
        Descricao = "Guia de boas práticas",
        ISBN = "978-0132350884",
        Autor = "Robert C. Martin",
        Editora = "Prentice Hall",
        Genero = GeneroLivroEnum.Ciencia,
        AnoDePublicacao = 2008,
        QuantidadePaginas = 464
    };

    // ── ObterLivros ────────────────────────────────────────────

    [Fact]
    public async Task ObterLivrosAsync_RetornaListaMapeada()
    {
        var livros = new List<Livro> { CriarLivroExemplo(1), CriarLivroExemplo(2) };
        _queryRepoMock.Setup(r => r.ObterLivrosAsync()).ReturnsAsync(livros);

        var resultado = await _sut.ObterLivrosAsync();

        Assert.Equal(2, resultado.Count());
    }

    // ── ObterLivroPorId ────────────────────────────────────────

    [Fact]
    public async Task ObterLivroPorIdAsync_Existente_RetornaViewModel()
    {
        var livro = CriarLivroExemplo();
        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(livro);

        var resultado = await _sut.ObterLivroPorIdAsync(new ObterLivroPorIdQuery(1));

        Assert.NotNull(resultado);
        Assert.Equal("Clean Code", resultado!.Titulo);
    }

    [Fact]
    public async Task ObterLivroPorIdAsync_Inexistente_RetornaNull()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(999)).ReturnsAsync((Livro?)null);

        var resultado = await _sut.ObterLivroPorIdAsync(new ObterLivroPorIdQuery(999));

        Assert.Null(resultado);
    }

    // ── CriarLivro ─────────────────────────────────────────────

    [Fact]
    public async Task CriarLivroAsync_Valido_RetornaId()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorISBNAsync("978-0132350884")).ReturnsAsync((Livro?)null);
        _commandRepoMock.Setup(r => r.CriarLivroAsync(It.IsAny<Livro>())).ReturnsAsync(10);

        var (sucesso, mensagem, id) = await _sut.CriarLivroAsync(CriarCommandValido());

        Assert.True(sucesso);
        Assert.Equal(10, id);
        Assert.Equal("Livro criado com sucesso.", mensagem);
    }

    [Fact]
    public async Task CriarLivroAsync_ISBNDuplicado_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorISBNAsync("978-0132350884")).ReturnsAsync(CriarLivroExemplo());

        var (sucesso, mensagem, _) = await _sut.CriarLivroAsync(CriarCommandValido());

        Assert.False(sucesso);
        Assert.Contains("ISBN", mensagem);
    }

    [Fact]
    public async Task CriarLivroAsync_TituloVazio_RetornaFalhaValidacao()
    {
        var cmd = CriarCommandValido();
        cmd.Titulo = "";

        var (sucesso, mensagem, _) = await _sut.CriarLivroAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Título é obrigatório.", mensagem);
    }

    // ── AtualizarLivro ─────────────────────────────────────────

    [Fact]
    public async Task AtualizarLivroAsync_Existente_RetornaSucesso()
    {
        var cmd = new AtualizarLivroCommand
        {
            Id = 1, Titulo = "Clean Code 2nd", ISBN = "978-0132350884",
            Autor = "Robert C. Martin", Editora = "Prentice Hall",
            AnoDePublicacao = 2020, QuantidadePaginas = 500
        };

        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(CriarLivroExemplo());
        _queryRepoMock.Setup(r => r.ObterLivroPorISBNAsync("978-0132350884")).ReturnsAsync(CriarLivroExemplo(1));
        _commandRepoMock.Setup(r => r.AtualizarLivroAsync(It.IsAny<Livro>())).ReturnsAsync(true);

        var (sucesso, _) = await _sut.AtualizarLivroAsync(cmd);

        Assert.True(sucesso);
    }

    [Fact]
    public async Task AtualizarLivroAsync_Inexistente_RetornaFalha()
    {
        var cmd = new AtualizarLivroCommand
        {
            Id = 999, Titulo = "X", ISBN = "123", Autor = "A", Editora = "E",
            AnoDePublicacao = 2020, QuantidadePaginas = 100
        };

        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(999)).ReturnsAsync((Livro?)null);

        var (sucesso, mensagem) = await _sut.AtualizarLivroAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Livro não encontrado.", mensagem);
    }

    [Fact]
    public async Task AtualizarLivroAsync_ISBNConflitoComOutro_RetornaFalha()
    {
        var cmd = new AtualizarLivroCommand
        {
            Id = 1, Titulo = "X", ISBN = "isbn-conflito", Autor = "A", Editora = "E",
            AnoDePublicacao = 2020, QuantidadePaginas = 100
        };

        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(CriarLivroExemplo(1));
        _queryRepoMock.Setup(r => r.ObterLivroPorISBNAsync("isbn-conflito")).ReturnsAsync(CriarLivroExemplo(2));

        var (sucesso, mensagem) = await _sut.AtualizarLivroAsync(cmd);

        Assert.False(sucesso);
        Assert.Contains("ISBN", mensagem);
    }

    // ── DeletarLivro ───────────────────────────────────────────

    [Fact]
    public async Task DeletarLivroAsync_Existente_RetornaSucesso()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(CriarLivroExemplo());
        _commandRepoMock.Setup(r => r.DeletarLivroAsync(1)).ReturnsAsync(true);

        var (sucesso, _) = await _sut.DeletarLivroAsync(1);

        Assert.True(sucesso);
    }

    [Fact]
    public async Task DeletarLivroAsync_Inexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(999)).ReturnsAsync((Livro?)null);

        var (sucesso, mensagem) = await _sut.DeletarLivroAsync(999);

        Assert.False(sucesso);
        Assert.Equal("Livro não encontrado.", mensagem);
    }

    // ── UploadCapa ─────────────────────────────────────────────

    [Fact]
    public async Task UploadCapaAsync_LivroExistente_RetornaSucesso()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(CriarLivroExemplo());
        _commandRepoMock.Setup(r => r.AtualizarCapaAsync(1, It.IsAny<byte[]>())).ReturnsAsync(true);

        var (sucesso, _) = await _sut.UploadCapaAsync(1, new byte[] { 1, 2, 3 });

        Assert.True(sucesso);
    }

    [Fact]
    public async Task UploadCapaAsync_LivroInexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterLivroPorIdAsync(999)).ReturnsAsync((Livro?)null);

        var (sucesso, mensagem) = await _sut.UploadCapaAsync(999, new byte[] { 1, 2, 3 });

        Assert.False(sucesso);
        Assert.Equal("Livro não encontrado.", mensagem);
    }

    // ── ObterCapa ──────────────────────────────────────────────

    [Fact]
    public async Task ObterCapaAsync_Existente_RetornaBytes()
    {
        var capa = new byte[] { 0xFF, 0xD8, 0xFF };
        _queryRepoMock.Setup(r => r.ObterCapaAsync(1)).ReturnsAsync(capa);

        var resultado = await _sut.ObterCapaAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal(3, resultado!.Length);
    }

    // ── ConsultarLivroExterno ──────────────────────────────────

    [Fact]
    public async Task ConsultarLivroExternoAsync_ISBNValido_RetornaDto()
    {
        var dto = new LivroExternoDto { Titulo = "Test", Autor = "Author" };
        _openLibraryMock.Setup(s => s.BuscarPorISBNAsync("123")).ReturnsAsync(dto);

        var resultado = await _sut.ConsultarLivroExternoAsync("123");

        Assert.NotNull(resultado);
        Assert.Equal("Test", resultado!.Titulo);
    }

    [Fact]
    public async Task ConsultarLivroExternoAsync_ISBNVazio_RetornaNull()
    {
        var resultado = await _sut.ConsultarLivroExternoAsync("");

        Assert.Null(resultado);
    }
}
