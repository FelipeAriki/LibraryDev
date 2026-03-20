using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Livros;

public interface ILivroCommandRepository
{
    Task<int> CriarLivroAsync(Livro livro);
    Task<bool> AtualizarLivroAsync(Livro livro);
    Task<bool> DeletarLivroAsync(int id);
    Task<bool> AtualizarCapaAsync(int id, byte[] capa);
    Task<bool> AtualizarNotaMediaAsync(int id, decimal notaMedia);
}
