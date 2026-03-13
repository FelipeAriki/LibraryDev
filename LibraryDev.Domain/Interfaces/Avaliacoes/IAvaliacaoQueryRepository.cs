using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Avaliacoes;

public interface IAvaliacaoQueryRepository
{
    Task<IEnumerable<Avaliacao>> ObterAvaliacoesAsync();
    Task<Avaliacao?> ObterAvaliacaoPorIdAsync(int id);
}
