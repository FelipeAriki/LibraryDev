using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Queries.Usuarios;
using LibraryDev.Application.ViewModels.Usuarios;

namespace LibraryDev.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<ObterUsuariosViewModel>> ObterUsuariosAsync();
    Task<ObterUsuarioPorIdViewModel> ObterUsuarioPorIdAsync(ObterUsuarioPorIdQuery query);
    Task<int> CriarUsuarioAsync(CriarUsuarioCommand command);
    Task<bool> AtualizarUsuarioAsync(AtualizarUsuarioCommand command);
    Task<bool> DeletarUsuarioAsync(int id);
}
