using LibraryDev.Application.Commands.Usuarios;
using System.Text.RegularExpressions;

namespace LibraryDev.Application.Validators.Usuarios;

public static class UsuarioValidator
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static (bool valido, string mensagem) ValidarCriar(CriarUsuarioCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Nome))
            return (false, "Nome é obrigatório.");

        if (command.Nome.Length > 200)
            return (false, "Nome não pode ultrapassar 200 caracteres.");

        if (string.IsNullOrWhiteSpace(command.Email))
            return (false, "E-mail é obrigatório.");

        if (!EmailRegex.IsMatch(command.Email))
            return (false, "E-mail inválido.");

        if (command.Email.Length > 320)
            return (false, "E-mail não pode ultrapassar 320 caracteres.");

        return (true, string.Empty);
    }

    public static (bool valido, string mensagem) ValidarAtualizar(AtualizarUsuarioCommand command)
    {
        if (command.Id <= 0)
            return (false, "Id inválido.");

        return ValidarCriar(new CriarUsuarioCommand
        (
            command.Nome,
            command.Email
        ));
    }
}
