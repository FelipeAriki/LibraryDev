using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Avaliacoes;

public interface IAvaliacaoCommandRepository
{
    Task<int> CriarAvaliacaoAsync(Avaliacao avaliacao);
    Task<bool> AtualizarAvaliacaoAsync(Avaliacao avaliacao);
    Task<bool> DeletarAvaliacaoAsync(int id);
}
