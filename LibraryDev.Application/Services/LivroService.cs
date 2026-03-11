using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Interfaces.Livros;
using LibraryDev.Application.Queries.Livros;
using LibraryDev.Application.ViewModels.Livros;
using LibraryDev.Domain.Interfaces.Livros;

namespace LibraryDev.Application.Services;

public class LivroService : ILivroService
{
    private readonly ILivroCommandRepository _livroCommandRepository;
    private readonly ILivroQueryRepository _livroQueryRepository;

    public LivroService(ILivroCommandRepository livroCommandRepository, ILivroQueryRepository livroQueryRepository)
    {
        _livroCommandRepository = livroCommandRepository;
        _livroQueryRepository = livroQueryRepository;
    }

    public async Task<IEnumerable<ObterLivrosViewModel>> ObterLivrosAsync()
    {
        var livros = await _livroQueryRepository.ObterLivrosAsync();
        return livros.Select(l => new ObterLivrosViewModel(l.Titulo, l.Descricao, l.ISBN, l.Autor, l.Editora, l.Genero, l.AnoDePublicacao, l.QuantidadePaginas, l.DataCriacao, l.NotaMedia, l.CapaLivro));
    }

    public async Task<ObterLivroPorIdViewModel> ObterLivroPorIdAsync(ObterLivroPorIdQuery query)
    {
        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(query.Id);
        return livro is null
            ? throw new KeyNotFoundException($"Livro com id {query.Id} não encontrado.")
            : new ObterLivroPorIdViewModel(
            livro.Titulo,
            livro.Descricao,
            livro.Autor,
            livro.Editora,
            livro.Genero);
    }

    public async Task<int> CriarLivroAsync(CriarLivroCommand command)
    {
        var idLivroCriado = await _livroCommandRepository.CriarLivroAsync(CriarLivroCommand.ToEntity(command));
        return idLivroCriado;
    }

    public async Task<bool> AtualizarLivroAsync(AtualizarLivroCommand livro)
    {
        var result = await _livroCommandRepository.AtualizarLivroAsync(AtualizarLivroCommand.ToEntity(livro));
        return result;
    }

    public async Task<bool> DeletarLivroAsync(int id)
    {
        var result = await _livroCommandRepository.DeletarLivroAsync(id);
        return result;
    }
}
