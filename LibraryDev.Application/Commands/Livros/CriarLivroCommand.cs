using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Enums;

namespace LibraryDev.Application.Commands.Livros;

public class CriarLivroCommand
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public GeneroLivroEnum Genero { get; set; }
    public int AnoDePublicacao { get; set; }
    public int QuantidadePaginas { get; set; }

    public static Livro ToEntity(CriarLivroCommand command)
    {
        return new Livro
        {
            Titulo = command.Titulo,
            Descricao = command.Descricao,
            ISBN = command.ISBN,
            Autor = command.Autor,
            Editora = command.Editora,
            Genero = command.Genero,
            AnoDePublicacao = command.AnoDePublicacao,
            QuantidadePaginas = command.QuantidadePaginas
        };
    }
}
