namespace LibraryDev.Application.Commands.Auth;

public class LoginCommand
{
    public string Email { get; set; }
    public string Senha { get; set; }

    public LoginCommand(string email, string senha)
    {
        Email = email;
        Senha = senha;
    }
}
