using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Commands.Usuarios;

public class AtualizarUsuarioCommand
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }

    public AtualizarUsuarioCommand(int id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
    }

    public static Usuario ToEntity(AtualizarUsuarioCommand command)
    {
        return new Usuario
        {
            Id = command.Id,
            Nome = command.Nome,
            Email = command.Email
        };
    }
}
