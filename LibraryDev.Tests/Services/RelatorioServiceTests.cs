using LibraryDev.Application.Queries.Relatorios;
using LibraryDev.Application.Services;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Relatorios;
using Moq;
using Xunit;

namespace LibraryDev.Tests.Services;

public class RelatorioServiceTests
{
    private readonly Mock<IRelatorioQueryRepository> _repoMock = new();
    private readonly RelatorioService _sut;

    public RelatorioServiceTests()
    {
        _sut = new RelatorioService(_repoMock.Object);
    }

    [Fact]
    public async Task ObterRelatorioLivrosLidosAsync_ComDados_RetornaRelatorioFormatado()
    {
        var avaliacoes = new List<Avaliacao>
        {
            new()
            {
                Id = 1, Nota = 5, IdUsuario = 1, IdLivro = 1,
                DataInicioLeitura = new DateTime(2025, 1, 1),
                DataFimLeitura = new DateTime(2025, 1, 11),
                DataCriacao = DateTime.UtcNow,
                Usuario = new Usuario { Nome = "Felipe" },
                Livro = new Livro { Titulo = "Clean Code", Autor = "Robert C. Martin" }
            },
            new()
            {
                Id = 2, Nota = 4, IdUsuario = 2, IdLivro = 2,
                DataInicioLeitura = new DateTime(2025, 3, 1),
                DataFimLeitura = new DateTime(2025, 3, 21),
                DataCriacao = DateTime.UtcNow,
                Usuario = new Usuario { Nome = "Ana" },
                Livro = new Livro { Titulo = "DDD", Autor = "Eric Evans" }
            }
        };

        _repoMock.Setup(r => r.ObterLivrosLidosPorAnoAsync(2025)).ReturnsAsync(avaliacoes);

        var resultado = await _sut.ObterRelatorioLivrosLidosAsync(new ObterRelatorioLivrosLidosQuery(2025));

        Assert.Equal(2025, resultado.Ano);
        Assert.Equal(2, resultado.TotalLivrosLidos);
        Assert.Equal(2, resultado.Livros.Count);
        Assert.Equal("Clean Code", resultado.Livros[0].TituloLivro);
        Assert.Equal(10, resultado.Livros[0].DiasLeitura);
        Assert.Equal(20, resultado.Livros[1].DiasLeitura);
    }

    [Fact]
    public async Task ObterRelatorioLivrosLidosAsync_SemDados_RetornaListaVazia()
    {
        _repoMock.Setup(r => r.ObterLivrosLidosPorAnoAsync(2020))
            .ReturnsAsync(new List<Avaliacao>());

        var resultado = await _sut.ObterRelatorioLivrosLidosAsync(new ObterRelatorioLivrosLidosQuery(2020));

        Assert.Equal(2020, resultado.Ano);
        Assert.Equal(0, resultado.TotalLivrosLidos);
        Assert.Empty(resultado.Livros);
    }
}
