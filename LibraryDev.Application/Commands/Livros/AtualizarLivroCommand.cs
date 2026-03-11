using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Enums;

namespace LibraryDev.Application.Commands.Livros;

public class AtualizarLivroCommand
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public GeneroLivroEnum Genero { get; set; }
    public int AnoDePublicacao { get; set; }
    public int QuantidadePaginas { get; set; }
    public DateTime DataCriacao { get; set; }
    public decimal NotaMedia { get; set; }
    public byte[] CapaLivro { get; set; } = [];

    public static Livro ToEntity(AtualizarLivroCommand command)
    {
        return new Livro
        {
            Id = command.Id,
            Titulo = command.Titulo,
            Descricao = command.Descricao,
            ISBN = command.ISBN,
            Autor = command.Autor,
            Editora = command.Editora,
            Genero = command.Genero,
            AnoDePublicacao = command.AnoDePublicacao,
            QuantidadePaginas = command.QuantidadePaginas,
            DataCriacao = command.DataCriacao,
            NotaMedia = command.NotaMedia,
            CapaLivro = command.CapaLivro
        };
    }
}
