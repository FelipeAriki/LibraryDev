using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Interfaces.Livros;

public interface ILivroQueryRepository
{
    Task<IEnumerable<Livro>> ObterLivrosAsync();
    Task<Livro?> ObterLivroPorIdAsync(int id);
}
