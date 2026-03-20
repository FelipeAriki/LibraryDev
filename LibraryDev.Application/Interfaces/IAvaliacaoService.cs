using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Queries.Avaliacoes;
using LibraryDev.Application.ViewModels.Avaliacoes;

namespace LibraryDev.Application.Interfaces;

public interface IAvaliacaoService
{
    Task<IEnumerable<ObterAvaliacoesViewModel>> ObterAvaliacoesAsync();
    Task<IEnumerable<ObterAvaliacoesViewModel>> ObterAvaliacoesPorLivroAsync(ObterAvaliacoesPorLivroQuery query);
    Task<ObterAvaliacaoPorIdViewModel?> ObterAvaliacaoPorIdAsync(ObterAvaliacaoPorIdQuery query);
    Task<(bool sucesso, string mensagem, int id)> CriarAvaliacaoAsync(CriarAvaliacaoCommand command);
    Task<(bool sucesso, string mensagem)> AtualizarAvaliacaoAsync(AtualizarAvaliacaoCommand command);
    Task<(bool sucesso, string mensagem)> DeletarAvaliacaoAsync(int id);
}
