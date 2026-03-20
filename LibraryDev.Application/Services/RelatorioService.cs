using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Relatorios;
using LibraryDev.Application.ViewModels.Relatorios;
using LibraryDev.Domain.Interfaces.Relatorios;

namespace LibraryDev.Application.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IRelatorioQueryRepository _relatorioQueryRepository;
    public RelatorioService(IRelatorioQueryRepository relatorioQueryRepository)
    {
        _relatorioQueryRepository = relatorioQueryRepository;
    }

    public async Task<RelatorioLivrosLidosViewModel> ObterRelatorioLivrosLidosAsync(ObterRelatorioLivrosLidosQuery query)
    {
        var avaliacoes = await _relatorioQueryRepository.ObterLivrosLidosPorAnoAsync(query.Ano);

        var itens = avaliacoes.Select(a => new LivroLidoItemViewModel
        {
            TituloLivro = a.Livro?.Titulo ?? string.Empty,
            Autor = a.Livro?.Autor ?? string.Empty,
            NomeUsuario = a.Usuario?.Nome ?? string.Empty,
            Nota = a.Nota,
            DataInicioLeitura = a.DataInicioLeitura,
            DataFimLeitura = a.DataFimLeitura,
            DiasLeitura = (int)(a.DataFimLeitura - a.DataInicioLeitura).TotalDays
        }).ToList();

        return new RelatorioLivrosLidosViewModel
        {
            Ano = query.Ano,
            TotalLivrosLidos = itens.Count,
            Livros = itens
        };
    }
}
