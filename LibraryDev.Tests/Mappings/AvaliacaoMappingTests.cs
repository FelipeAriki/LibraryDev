using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Mappings.Avaliacoes;
using LibraryDev.Domain.Entities;
using Xunit;

namespace LibraryDev.Tests.Mappings;

public class AvaliacaoMappingTests
{
    [Fact]
    public void MapToViewModel_DeveMapearTodosOsCampos()
    {
        var avaliacao = new Avaliacao
        {
            Id = 1,
            Nota = 5,
            Descricao = "Excelente",
            IdUsuario = 10,
            IdLivro = 20,
            DataInicioLeitura = new DateTime(2025, 1, 1),
            DataFimLeitura = new DateTime(2025, 1, 15),
            DataCriacao = new DateTime(2025, 1, 16),
            Usuario = new Usuario { Id = 10, Nome = "João" },
            Livro = new Livro { Id = 20, Titulo = "Clean Code" }
        };

        var vm = AvaliacaoMapping.MapToViewModel(avaliacao);

        Assert.Equal(1, vm.Id);
        Assert.Equal(5, vm.Nota);
        Assert.Equal("Excelente", vm.Descricao);
        Assert.Equal(10, vm.IdUsuario);
        Assert.Equal("João", vm.NomeUsuario);
        Assert.Equal(20, vm.IdLivro);
        Assert.Equal("Clean Code", vm.TituloLivro);
        Assert.Equal(new DateTime(2025, 1, 1), vm.DataInicioLeitura);
        Assert.Equal(new DateTime(2025, 1, 15), vm.DataFimLeitura);
        Assert.Equal(new DateTime(2025, 1, 16), vm.DataCriacao);
    }

    [Fact]
    public void MapToViewModel_SemUsuarioNemLivro_RetornaVazio()
    {
        var avaliacao = new Avaliacao
        {
            Id = 2,
            Nota = 3,
            IdUsuario = 1,
            IdLivro = 1,
            DataInicioLeitura = new DateTime(2025, 2, 1),
            DataFimLeitura = new DateTime(2025, 2, 10),
            DataCriacao = new DateTime(2025, 2, 11)
        };

        var vm = AvaliacaoMapping.MapToViewModel(avaliacao);

        Assert.Equal(string.Empty, vm.NomeUsuario);
        Assert.Equal(string.Empty, vm.TituloLivro);
    }

    [Fact]
    public void ToEntity_DeveMapearAtualizarCommandParaEntidade()
    {
        var command = new AtualizarAvaliacaoCommand
        {
            Id = 5,
            Nota = 4,
            Descricao = "Muito bom",
            IdUsuario = 10,
            IdLivro = 20,
            DataInicioLeitura = new DateTime(2025, 3, 1),
            DataFimLeitura = new DateTime(2025, 3, 15)
        };

        var entity = AvaliacaoMapping.ToEntity(command);

        Assert.Equal(5, entity.Id);
        Assert.Equal(4, entity.Nota);
        Assert.Equal("Muito bom", entity.Descricao);
        Assert.Equal(10, entity.IdUsuario);
        Assert.Equal(20, entity.IdLivro);
        Assert.Equal(new DateTime(2025, 3, 1), entity.DataInicioLeitura);
        Assert.Equal(new DateTime(2025, 3, 15), entity.DataFimLeitura);
    }
}
