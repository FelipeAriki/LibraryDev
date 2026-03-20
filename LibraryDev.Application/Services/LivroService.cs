using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Interfaces.Livros;
using LibraryDev.Application.Queries.Livros;
using LibraryDev.Application.Validators.Livros;
using LibraryDev.Application.ViewModels.Livros;
using LibraryDev.Domain.Interfaces.Livros;
using LibraryDev.Domain.Services;

namespace LibraryDev.Application.Services;

public class LivroService : ILivroService
{
    private readonly ILivroCommandRepository _livroCommandRepository;
    private readonly ILivroQueryRepository _livroQueryRepository;
    private readonly IOpenLibraryService _openLibraryService;

    public LivroService(
        ILivroCommandRepository commandRepo,
        ILivroQueryRepository queryRepo,
        IOpenLibraryService openLibraryService)
    {
        _livroCommandRepository = commandRepo;
        _livroQueryRepository = queryRepo;
        _openLibraryService = openLibraryService;
    }

    public async Task<IEnumerable<ObterLivrosViewModel>> ObterLivrosAsync()
    {
        var livros = await _livroQueryRepository.ObterLivrosAsync();
        return livros.Select(l => new ObterLivrosViewModel
        (
            l.Id,
            l.Titulo,
            l.Descricao,
            l.ISBN,
            l.Autor,
            l.Editora,
            l.Genero.ToString(),
            l.AnoDePublicacao,
            l.QuantidadePaginas,
            l.DataCriacao,
            l.NotaMedia
        ));
    }

    public async Task<ObterLivroPorIdViewModel?> ObterLivroPorIdAsync(ObterLivroPorIdQuery query)
    {
        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(query.Id);
        if (livro is null) return null;

        return new ObterLivroPorIdViewModel
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Descricao = livro.Descricao,
            ISBN = livro.ISBN,
            Autor = livro.Autor,
            Editora = livro.Editora,
            Genero = livro.Genero.ToString(),
            AnoDePublicacao = livro.AnoDePublicacao,
            QuantidadePaginas = livro.QuantidadePaginas,
            DataCriacao = livro.DataCriacao,
            NotaMedia = livro.NotaMedia,
            TemCapa = livro.CapaLivro != null && livro.CapaLivro.Length > 0,
            Avaliacoes = livro.Avaliacoes.Select(a => new AvaliacaoResumidaViewModel
            {
                Id = a.Id,
                Nota = a.Nota,
                Descricao = a.Descricao,
                NomeUsuario = a.Usuario?.Nome ?? string.Empty,
                DataCriacao = a.DataCriacao
            }).ToList()
        };
    }

    public async Task<(bool sucesso, string mensagem, int id)> CriarLivroAsync(CriarLivroCommand command)
    {
        var (valido, mensagem) = LivroValidator.ValidarCriar(command);
        if (!valido) return (false, mensagem, 0);

        var isbnExistente = await _livroQueryRepository.ObterLivroPorISBNAsync(command.ISBN);
        if (isbnExistente is not null)
            return (false, $"Já existe um livro cadastrado com o ISBN '{command.ISBN}'.", 0);

        var id = await _livroCommandRepository.CriarLivroAsync(CriarLivroCommand.ToEntity(command));
        return (true, "Livro criado com sucesso.", id);
    }

    public async Task<(bool sucesso, string mensagem)> AtualizarLivroAsync(AtualizarLivroCommand command)
    {
        var (valido, mensagem) = LivroValidator.ValidarAtualizar(command);
        if (!valido) return (false, mensagem);

        var livroExistente = await _livroQueryRepository.ObterLivroPorIdAsync(command.Id);
        if (livroExistente is null) return (false, "Livro não encontrado.");

        // ISBN só pode conflitar com outro livro (não consigo próprio)
        var isbnConflito = await _livroQueryRepository.ObterLivroPorISBNAsync(command.ISBN);
        if (isbnConflito is not null && isbnConflito.Id != command.Id)
            return (false, $"Já existe outro livro cadastrado com o ISBN '{command.ISBN}'.");

        var resultado = await _livroCommandRepository.AtualizarLivroAsync(AtualizarLivroCommand.ToEntity(command));
        return resultado
            ? (true, "Livro atualizado com sucesso.")
            : (false, "Não foi possível atualizar o livro.");
    }

    public async Task<(bool sucesso, string mensagem)> DeletarLivroAsync(int id)
    {
        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(id);
        if (livro is null) return (false, "Livro não encontrado.");

        var resultado = await _livroCommandRepository.DeletarLivroAsync(id);
        return resultado
            ? (true, "Livro removido com sucesso.")
            : (false, "Não foi possível remover o livro.");
    }

    public async Task<(bool sucesso, string mensagem)> UploadCapaAsync(int id, byte[] capa)
    {
        if (capa == null || capa.Length == 0)
            return (false, "Arquivo de capa inválido.");

        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(id);
        if (livro is null) return (false, "Livro não encontrado.");

        var resultado = await _livroCommandRepository.AtualizarCapaAsync(id, capa);
        return resultado
            ? (true, "Capa atualizada com sucesso.")
            : (false, "Não foi possível atualizar a capa.");
    }

    public async Task<byte[]?> ObterCapaAsync(int id)
    {
        var livro = await _livroQueryRepository.ObterLivroPorIdAsync(id);
        return livro?.CapaLivro;
    }

    public async Task<LivroExternoViewModel?> ConsultarLivroExternoAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return null;
        return await _openLibraryService.BuscarPorISBNAsync(isbn);
    }
}
