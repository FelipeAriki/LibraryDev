using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Avaliacoes;

public interface IAvaliacaoQueryRepository
{
    Task<IEnumerable<Avaliacao>> ObterAvaliacoesAsync();
    Task<IEnumerable<Avaliacao>> ObterAvaliacoesPorLivroAsync(int idLivro);
    Task<Avaliacao?> ObterAvaliacaoPorIdAsync(int id);
    Task<decimal> CalcularNotaMediaLivroAsync(int idLivro);
}
