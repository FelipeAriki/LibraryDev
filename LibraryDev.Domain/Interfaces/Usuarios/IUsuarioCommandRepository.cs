using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Usuarios;

public interface IUsuarioCommandRepository
{
        Task<int> CriarUsuarioAsync(Usuario usuario);
        Task<bool> AtualizarUsuarioAsync(Usuario usuario);
        Task<bool> DeletarUsuarioAsync(int id);
}
