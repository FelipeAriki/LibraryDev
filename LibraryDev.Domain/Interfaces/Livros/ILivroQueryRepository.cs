using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Livros;

public interface ILivroQueryRepository
{
    Task<IEnumerable<Livro>> ObterLivrosAsync();
    Task<Livro?> ObterLivroPorIdAsync(int id);
    Task<Livro?> ObterLivroPorISBNAsync(string isbn);
    Task<byte[]?> ObterCapaAsync(int id);
}
