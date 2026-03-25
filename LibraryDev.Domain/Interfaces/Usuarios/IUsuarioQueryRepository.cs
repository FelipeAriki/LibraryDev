using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Usuarios;

public record UsuarioResumo(int Id, string Nome, string Email, int TotalAvaliacoes);

public interface IUsuarioQueryRepository
{
    Task<IEnumerable<UsuarioResumo>> ObterUsuariosAsync();
    Task<Usuario?> ObterUsuarioPorIdAsync(int id);
    Task<Usuario?> ObterUsuarioComAvaliacoesPorIdAsync(int id);
    Task<bool> EmailJaCadastradoAsync(string email, int? ignorarId = null);
    Task<Usuario?> ObterUsuarioPorEmailAsync(string email);
    Task<Usuario?> ObterUsuarioPorRefreshTokenAsync(string refreshToken);
    Task<Usuario?> ObterUsuarioPorTokenRecuperacaoAsync(string token);
}
