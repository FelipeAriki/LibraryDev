using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Queries.Livros;
using LibraryDev.Application.ViewModels.Livros;
using LibraryDev.Domain.Services;

namespace LibraryDev.Application.Interfaces;

public interface ILivroService
{
    Task<IEnumerable<ObterLivrosViewModel>> ObterLivrosAsync();
    Task<ObterLivroPorIdViewModel?> ObterLivroPorIdAsync(ObterLivroPorIdQuery query);
    Task<(bool sucesso, string mensagem, int id)> CriarLivroAsync(CriarLivroCommand command);
    Task<(bool sucesso, string mensagem)> AtualizarLivroAsync(AtualizarLivroCommand command);
    Task<(bool sucesso, string mensagem)> DeletarLivroAsync(int id);
    Task<(bool sucesso, string mensagem)> UploadCapaAsync(int id, byte[] capa);
    Task<byte[]?> ObterCapaAsync(int id);
    Task<LivroExternoViewModel?> ConsultarLivroExternoAsync(string isbn);
}
