using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Queries.Avaliacoes;
using LibraryDev.Application.Services;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Avaliacoes;
using LibraryDev.Domain.Interfaces.Livros;
using LibraryDev.Domain.Interfaces.Usuarios;
using Moq;
using Xunit;

namespace LibraryDev.Tests.Services;

public class AvaliacaoServiceTests
{
    private readonly Mock<IAvaliacaoCommandRepository> _commandRepoMock = new();
    private readonly Mock<IAvaliacaoQueryRepository> _queryRepoMock = new();
    private readonly Mock<ILivroQueryRepository> _livroQueryMock = new();
    private readonly Mock<ILivroCommandRepository> _livroCommandMock = new();
    private readonly Mock<IUsuarioQueryRepository> _usuarioQueryMock = new();
    private readonly AvaliacaoService _sut;

    public AvaliacaoServiceTests()
    {
        _sut = new AvaliacaoService(
            _commandRepoMock.Object,
            _queryRepoMock.Object,
            _livroQueryMock.Object,
            _livroCommandMock.Object,
            _usuarioQueryMock.Object);
    }

    private static CriarAvaliacaoCommand CriarCommandValido() => new()
    {
        Nota = 4,
        Descricao = "Bom livro",
        IdUsuario = 1,
        IdLivro = 1,
        DataInicioLeitura = new DateTime(2025, 1, 1),
        DataFimLeitura = new DateTime(2025, 1, 15)
    };

    // ── ObterAvaliacoes ────────────────────────────────────────

    [Fact]
    public async Task ObterAvaliacoesAsync_RetornaListaMapeada()
    {
        var avaliacoes = new List<Avaliacao>
        {
            new() { Id = 1, Nota = 5, IdUsuario = 1, IdLivro = 1, DataInicioLeitura = DateTime.Now, DataFimLeitura = DateTime.Now, DataCriacao = DateTime.Now, Usuario = new Usuario { Nome = "User" }, Livro = new Livro { Titulo = "Book" } }
        };
        _queryRepoMock.Setup(r => r.ObterAvaliacoesAsync()).ReturnsAsync(avaliacoes);

        var resultado = await _sut.ObterAvaliacoesAsync();

        Assert.Single(resultado);
    }

    // ── ObterAvaliacoesPorLivro ────────────────────────────────

    [Fact]
    public async Task ObterAvaliacoesPorLivroAsync_RetornaFiltrado()
    {
        var avaliacoes = new List<Avaliacao>
        {
            new() { Id = 1, Nota = 5, IdUsuario = 1, IdLivro = 10, DataInicioLeitura = DateTime.Now, DataFimLeitura = DateTime.Now, DataCriacao = DateTime.Now, Usuario = new Usuario { Nome = "U" }, Livro = new Livro { Titulo = "L" } }
        };
        _queryRepoMock.Setup(r => r.ObterAvaliacoesPorLivroAsync(10)).ReturnsAsync(avaliacoes);

        var resultado = await _sut.ObterAvaliacoesPorLivroAsync(new ObterAvaliacoesPorLivroQuery(10));

        Assert.Single(resultado);
    }

    // ── ObterAvaliacaoPorId ────────────────────────────────────

    [Fact]
    public async Task ObterAvaliacaoPorIdAsync_Existente_RetornaViewModel()
    {
        var avaliacao = new Avaliacao
        {
            Id = 1, Nota = 5, IdUsuario = 1, IdLivro = 1,
            DataInicioLeitura = new DateTime(2025, 1, 1),
            DataFimLeitura = new DateTime(2025, 1, 15),
            DataCriacao = DateTime.UtcNow,
            Usuario = new Usuario { Nome = "User" },
            Livro = new Livro { Titulo = "Book" }
        };
        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(1)).ReturnsAsync(avaliacao);

        var resultado = await _sut.ObterAvaliacaoPorIdAsync(new ObterAvaliacaoPorIdQuery(1));

        Assert.NotNull(resultado);
        Assert.Equal(5, resultado!.Nota);
    }

    [Fact]
    public async Task ObterAvaliacaoPorIdAsync_Inexistente_RetornaNull()
    {
        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(999)).ReturnsAsync((Avaliacao?)null);

        var resultado = await _sut.ObterAvaliacaoPorIdAsync(new ObterAvaliacaoPorIdQuery(999));

        Assert.Null(resultado);
    }

    // ── CriarAvaliacao ─────────────────────────────────────────

    [Fact]
    public async Task CriarAvaliacaoAsync_Valida_RetornaIdERecalculaMedia()
    {
        var cmd = CriarCommandValido();

        _livroQueryMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(new Livro { Id = 1 });
        _usuarioQueryMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(new Usuario { Id = 1 });
        _commandRepoMock.Setup(r => r.CriarAvaliacaoAsync(It.IsAny<Avaliacao>())).ReturnsAsync(10);
        _queryRepoMock.Setup(r => r.CalcularNotaMediaLivroAsync(1)).ReturnsAsync(4.5m);
        _livroCommandMock.Setup(r => r.AtualizarNotaMediaAsync(1, 4.5m)).ReturnsAsync(true);

        var (sucesso, _, id) = await _sut.CriarAvaliacaoAsync(cmd);

        Assert.True(sucesso);
        Assert.Equal(10, id);
        _livroCommandMock.Verify(r => r.AtualizarNotaMediaAsync(1, 4.5m), Times.Once);
    }

    [Fact]
    public async Task CriarAvaliacaoAsync_LivroInexistente_RetornaFalha()
    {
        var cmd = CriarCommandValido();
        _livroQueryMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync((Livro?)null);

        var (sucesso, mensagem, _) = await _sut.CriarAvaliacaoAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Livro não encontrado.", mensagem);
    }

    [Fact]
    public async Task CriarAvaliacaoAsync_UsuarioInexistente_RetornaFalha()
    {
        var cmd = CriarCommandValido();
        _livroQueryMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(new Livro { Id = 1 });
        _usuarioQueryMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync((Usuario?)null);

        var (sucesso, mensagem, _) = await _sut.CriarAvaliacaoAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Usuário não encontrado.", mensagem);
    }

    [Fact]
    public async Task CriarAvaliacaoAsync_NotaForaDoRange_RetornaFalha()
    {
        var cmd = CriarCommandValido();
        cmd.Nota = 0;

        var (sucesso, mensagem, _) = await _sut.CriarAvaliacaoAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Nota deve ser entre 1 e 5.", mensagem);
    }

    [Fact]
    public async Task CriarAvaliacaoAsync_DataInicioMaiorQueDataFim_RetornaFalha()
    {
        var cmd = CriarCommandValido();
        cmd.DataInicioLeitura = new DateTime(2025, 2, 1);
        cmd.DataFimLeitura = new DateTime(2025, 1, 1);

        var (sucesso, mensagem, _) = await _sut.CriarAvaliacaoAsync(cmd);

        Assert.False(sucesso);
        Assert.Contains("Data de início", mensagem);
    }

    // ── AtualizarAvaliacao ─────────────────────────────────────

    [Fact]
    public async Task AtualizarAvaliacaoAsync_Existente_RetornaSucessoERecalculaMedia()
    {
        var cmd = new AtualizarAvaliacaoCommand
        {
            Id = 1, Nota = 3, IdUsuario = 1, IdLivro = 1,
            DataInicioLeitura = new DateTime(2025, 1, 1),
            DataFimLeitura = new DateTime(2025, 1, 15)
        };

        var avaliacao = new Avaliacao { Id = 1, IdLivro = 1, Usuario = new(), Livro = new() };
        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(1)).ReturnsAsync(avaliacao);
        _livroQueryMock.Setup(r => r.ObterLivroPorIdAsync(1)).ReturnsAsync(new Livro { Id = 1 });
        _usuarioQueryMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(new Usuario { Id = 1 });
        _commandRepoMock.Setup(r => r.AtualizarAvaliacaoAsync(It.IsAny<Avaliacao>())).ReturnsAsync(true);
        _queryRepoMock.Setup(r => r.CalcularNotaMediaLivroAsync(1)).ReturnsAsync(3.5m);
        _livroCommandMock.Setup(r => r.AtualizarNotaMediaAsync(1, 3.5m)).ReturnsAsync(true);

        var (sucesso, _) = await _sut.AtualizarAvaliacaoAsync(cmd);

        Assert.True(sucesso);
        _livroCommandMock.Verify(r => r.AtualizarNotaMediaAsync(1, 3.5m), Times.Once);
    }

    [Fact]
    public async Task AtualizarAvaliacaoAsync_Inexistente_RetornaFalha()
    {
        var cmd = new AtualizarAvaliacaoCommand
        {
            Id = 999, Nota = 3, IdUsuario = 1, IdLivro = 1,
            DataInicioLeitura = new DateTime(2025, 1, 1),
            DataFimLeitura = new DateTime(2025, 1, 15)
        };

        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(999)).ReturnsAsync((Avaliacao?)null);

        var (sucesso, mensagem) = await _sut.AtualizarAvaliacaoAsync(cmd);

        Assert.False(sucesso);
        Assert.Equal("Avaliação não encontrada.", mensagem);
    }

    // ── DeletarAvaliacao ───────────────────────────────────────

    [Fact]
    public async Task DeletarAvaliacaoAsync_Existente_RetornaSucessoERecalculaMedia()
    {
        var avaliacao = new Avaliacao { Id = 1, IdLivro = 5 };
        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(1)).ReturnsAsync(avaliacao);
        _commandRepoMock.Setup(r => r.DeletarAvaliacaoAsync(1)).ReturnsAsync(true);
        _queryRepoMock.Setup(r => r.CalcularNotaMediaLivroAsync(5)).ReturnsAsync(0m);
        _livroCommandMock.Setup(r => r.AtualizarNotaMediaAsync(5, 0m)).ReturnsAsync(true);

        var (sucesso, _) = await _sut.DeletarAvaliacaoAsync(1);

        Assert.True(sucesso);
        _livroCommandMock.Verify(r => r.AtualizarNotaMediaAsync(5, 0m), Times.Once);
    }

    [Fact]
    public async Task DeletarAvaliacaoAsync_Inexistente_RetornaFalha()
    {
        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(999)).ReturnsAsync((Avaliacao?)null);

        var (sucesso, mensagem) = await _sut.DeletarAvaliacaoAsync(999);

        Assert.False(sucesso);
        Assert.Equal("Avaliação não encontrada.", mensagem);
    }

    [Fact]
    public async Task DeletarAvaliacaoAsync_FalhaAoRemover_RetornaFalha()
    {
        var avaliacao = new Avaliacao { Id = 1, IdLivro = 5 };
        _queryRepoMock.Setup(r => r.ObterAvaliacaoPorIdAsync(1)).ReturnsAsync(avaliacao);
        _commandRepoMock.Setup(r => r.DeletarAvaliacaoAsync(1)).ReturnsAsync(false);

        var (sucesso, mensagem) = await _sut.DeletarAvaliacaoAsync(1);

        Assert.False(sucesso);
        Assert.Equal("Não foi possível remover a avaliação.", mensagem);
    }
}
