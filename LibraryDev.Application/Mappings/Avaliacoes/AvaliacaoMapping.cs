using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.ViewModels.Avaliacoes;
using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Mappings.Avaliacoes;

public static class AvaliacaoMapping
{
    public static ObterAvaliacoesViewModel MapToViewModel(Avaliacao a) => new(
        a.Id,
        a.Nota,
        a.Descricao,
        a.IdUsuario,
        a.Usuario?.Nome ?? string.Empty,
        a.IdLivro,
        a.Livro?.Titulo ?? string.Empty,
        a.DataInicioLeitura,
        a.DataFimLeitura,
        a.DataCriacao
    );

    public static Avaliacao ToEntity(AtualizarAvaliacaoCommand command) => new()
    {
        Id = command.Id,
        Nota = command.Nota,
        Descricao = command.Descricao,
        IdUsuario = command.IdUsuario,
        IdLivro = command.IdLivro,
        DataInicioLeitura = command.DataInicioLeitura,
        DataFimLeitura = command.DataFimLeitura
    };
}
