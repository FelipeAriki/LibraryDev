using LibraryDev.Application.Commands.Auth;
using LibraryDev.Application.ViewModels.Auth;

namespace LibraryDev.Application.Interfaces;

public interface IAuthService
{
    Task<(bool sucesso, string mensagem, LoginViewModel? resultado)> LoginAsync(LoginCommand command);
    Task<(bool sucesso, string mensagem, TokenRefreshViewModel? resultado)> RefreshTokenAsync(RefreshTokenCommand command);
    Task<(bool sucesso, string mensagem)> RecuperarSenhaAsync(RecuperarSenhaCommand command);
    Task<(bool sucesso, string mensagem)> RedefinirSenhaAsync(RedefinirSenhaCommand command);
}
