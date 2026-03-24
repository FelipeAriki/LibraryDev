namespace LibraryDev.Application.Commands.Auth;

public class RedefinirSenhaCommand
{
    public string Token { get; set; }
    public string NovaSenha { get; set; }

    public RedefinirSenhaCommand(string token, string novaSenha)
    {
        Token = token;
        NovaSenha = novaSenha;
    }
}
