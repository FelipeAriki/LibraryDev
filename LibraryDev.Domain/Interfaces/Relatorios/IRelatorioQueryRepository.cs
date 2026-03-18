using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Relatorios;

public interface IRelatorioQueryRepository
{
    Task<IEnumerable<Avaliacao>> ObterLivrosLidosPorAnoAsync(int ano);
}
