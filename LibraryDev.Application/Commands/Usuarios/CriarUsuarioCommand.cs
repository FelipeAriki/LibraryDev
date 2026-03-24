using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Commands.Usuarios;

public class CriarUsuarioCommand
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }

    public CriarUsuarioCommand(string nome, string email, string senha)
    {
        Nome = nome;
        Email = email;
        Senha = senha;
    }

    public static Usuario ToEntity(CriarUsuarioCommand command)
    {
        return new Usuario
        {
            Nome = command.Nome,
            Email = command.Email,
            Senha = command.Senha
        };
    }
}
