using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Validators.Auth;
using System.Text.RegularExpressions;

namespace LibraryDev.Application.Validators.Usuarios;

public static class UsuarioValidator
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static (bool valido, string mensagem) ValidarCriar(CriarUsuarioCommand command)
    {
        var (nomeValido, mensagemNome) = ValidarNomeEmail(command.Nome, command.Email);
        if (!nomeValido) return (false, mensagemNome);

        var (senhaValida, mensagemSenha) = AuthValidator.ValidarSenha(command.Senha);
        if (!senhaValida)
            return (false, mensagemSenha);

        return (true, string.Empty);
    }

    public static (bool valido, string mensagem) ValidarAtualizar(AtualizarUsuarioCommand command)
    {
        if (command.Id <= 0)
            return (false, "Id inválido.");

        return ValidarNomeEmail(command.Nome, command.Email);
    }

    private static (bool valido, string mensagem) ValidarNomeEmail(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return (false, "Nome é obrigatório.");

        if (nome.Length > 200)
            return (false, "Nome não pode ultrapassar 200 caracteres.");

        if (string.IsNullOrWhiteSpace(email))
            return (false, "E-mail é obrigatório.");

        if (!EmailRegex.IsMatch(email))
            return (false, "E-mail inválido.");

        if (email.Length > 320)
            return (false, "E-mail não pode ultrapassar 320 caracteres.");

        return (true, string.Empty);
    }
}
