using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Usuarios;

public interface IUsuarioQueryRepository
{
    Task<IEnumerable<Usuario>> ObterUsuariosAsync();
    Task<Usuario?> ObterUsuarioPorIdAsync(int id);
}
