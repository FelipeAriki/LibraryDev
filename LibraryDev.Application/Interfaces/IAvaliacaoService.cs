using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Queries.Avaliacoes;
using LibraryDev.Application.ViewModels.Avaliacoes;

namespace LibraryDev.Application.Interfaces;

public interface IAvaliacaoService
{
    Task<IEnumerable<ObterAvaliacoesViewModel>> ObterAvaliacoes();
    Task<ObterAvalicaoPorIdViewModel> ObterAvalicaoPorId(ObterAvaliacaoPorIdQuery query);
    Task<int> CriarAvaliacaoAsync(CriarAvaliacaoCommand command);
    Task<bool> AtualizarAvaliacaoAsync(AtualizarAvaliacaoCommand avaliacao);
    Task<bool> DeletarAvaliacaoAsync(int id);
}
