using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Usuarios;
using LibraryDev.Application.ViewModels.Usuarios;
using LibraryDev.Domain.Interfaces.Usuarios;

namespace LibraryDev.Application.Services;

public class UsuarioService : IUsuarioService
{

    private readonly IUsuarioCommandRepository _usuarioCommandRepository;
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;

    public UsuarioService(IUsuarioCommandRepository usuarioCommandRepository, IUsuarioQueryRepository usuarioQueryRepository)
    {
        _usuarioCommandRepository = usuarioCommandRepository;
        _usuarioQueryRepository = usuarioQueryRepository;
    }

    public async Task<IEnumerable<ObterUsuariosViewModel>> ObterUsuariosAsync()
    {
        var usuarios = await _usuarioQueryRepository.ObterUsuariosAsync();
        return usuarios.Select(u => new ObterUsuariosViewModel(u.Id, u.Nome, u.Email));
    }

    public async Task<ObterUsuarioPorIdViewModel> ObterUsuarioPorIdAsync(ObterUsuarioPorIdQuery query)
    {
        var usuario = await _usuarioQueryRepository.ObterUsuarioPorIdAsync(query.Id);
        if (usuario == null)
            return null;
        return new ObterUsuarioPorIdViewModel(usuario.Id, usuario.Nome, usuario.Email);
    }

    public async Task<int> CriarUsuarioAsync(CriarUsuarioCommand command)
    {
        var idUsuarioCriado = await _usuarioCommandRepository.CriarUsuarioAsync(CriarUsuarioCommand.ToEntity(command));
        return idUsuarioCriado;
    }

    public async Task<bool> AtualizarUsuarioAsync(AtualizarUsuarioCommand command)
    {
        var usuarioAtualizado = await _usuarioCommandRepository.AtualizarUsuarioAsync(AtualizarUsuarioCommand.ToEntity(command));
        return usuarioAtualizado;
    }

    public async Task<bool> DeletarUsuarioAsync(int id)
    {
        var usuarioDeletado = await _usuarioCommandRepository.DeletarUsuarioAsync(id);
        return usuarioDeletado;
    }
}
