using LibraryDev.Domain.Entities;

namespace LibraryDev.Domain.Interfaces.Usuarios;

public interface IUsuarioCommandRepository
{
        Task<int> CriarUsuarioAsync(Usuario usuario);
        Task<bool> AtualizarUsuarioAsync(Usuario usuario);
        Task<bool> DeletarUsuarioAsync(int id);
        Task<bool> AtualizarRefreshTokenAsync(int id, string? refreshToken, DateTime? expiracao);
        Task<bool> AtualizarTokenRecuperacaoAsync(int id, string? token, DateTime? expiracao);
        Task<bool> AtualizarSenhaAsync(int id, string senhaHash);
}
