using LibraryDev.Application.Commands.Auth;

namespace LibraryDev.Application.Validators.Auth;

public static class AuthValidator
{
    public static (bool valido, string mensagem) ValidarLogin(LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return (false, "E-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(command.Senha))
            return (false, "Senha é obrigatória.");

        return (true, string.Empty);
    }

    public static (bool valido, string mensagem) ValidarSenha(string? senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return (false, "Senha é obrigatória.");

        if (senha.Length < 8)
            return (false, "Senha deve ter no mínimo 8 caracteres.");

        if (!senha.Any(char.IsUpper))
            return (false, "Senha deve conter pelo menos uma letra maiúscula.");

        if (!senha.Any(char.IsLower))
            return (false, "Senha deve conter pelo menos uma letra minúscula.");

        if (!senha.Any(char.IsDigit))
            return (false, "Senha deve conter pelo menos um número.");

        if (!senha.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Senha deve conter pelo menos um caractere especial.");

        return (true, string.Empty);
    }

    public static (bool valido, string mensagem) ValidarRedefinirSenha(RedefinirSenhaCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Token))
            return (false, "Token é obrigatório.");

        return ValidarSenha(command.NovaSenha);
    }
}
