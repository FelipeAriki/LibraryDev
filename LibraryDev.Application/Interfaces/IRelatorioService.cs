using LibraryDev.Application.Queries.Relatorios;
using LibraryDev.Application.ViewModels.Relatorios;

namespace LibraryDev.Application.Interfaces;

public interface IRelatorioService
{
    Task<RelatorioLivrosLidosViewModel> ObterRelatorioLivrosLidosAsync(ObterRelatorioLivrosLidosQuery query);
}
