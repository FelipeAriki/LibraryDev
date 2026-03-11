using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Queries.Livros;
using LibraryDev.Application.ViewModels.Livros;

namespace LibraryDev.Application.Interfaces;

public interface ILivroService
{
    Task<IEnumerable<ObterLivrosViewModel>> ObterLivrosAsync();
    Task<ObterLivroPorIdViewModel> ObterLivroPorIdAsync(ObterLivroPorIdQuery query);
    Task<int> CriarLivroAsync(CriarLivroCommand command);
    Task<bool> AtualizarLivroAsync(AtualizarLivroCommand livro);
    Task<bool> DeletarLivroAsync(int id);
}
