namespace LibraryDev.Application.Commands.Auth;

public class RefreshTokenCommand
{
    public string RefreshToken { get; set; }

    public RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
