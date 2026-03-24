namespace LibraryDev.Application.Commands.Auth;

public class RecuperarSenhaCommand
{
    public string Email { get; set; }

    public RecuperarSenhaCommand(string email)
    {
        Email = email;
    }
}
