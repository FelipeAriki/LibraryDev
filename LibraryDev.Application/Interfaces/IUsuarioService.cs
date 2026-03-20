using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Queries.Usuarios;
using LibraryDev.Application.ViewModels.Usuarios;

namespace LibraryDev.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<ObterUsuariosViewModel>> ObterUsuariosAsync();
    Task<ObterUsuarioPorIdViewModel?> ObterUsuarioPorIdAsync(ObterUsuarioPorIdQuery query);
    Task<(bool sucesso, string mensagem, int id)> CriarUsuarioAsync(CriarUsuarioCommand command);
    Task<(bool sucesso, string mensagem)> AtualizarUsuarioAsync(AtualizarUsuarioCommand command);
    Task<(bool sucesso, string mensagem)> DeletarUsuarioAsync(int id);
}
