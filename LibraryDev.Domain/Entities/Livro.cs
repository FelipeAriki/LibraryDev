using LibraryDev.Domain.Enums;

namespace LibraryDev.Domain.Entities;

public class Livro
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
    public byte[]? CapaLivro { get; set; }
    public List<Avaliacao> Avaliacoes { get; set; } = [];
}
