using LibraryDev.Domain.Enums;

namespace LibraryDev.Application.ViewModels.Livros;

public class ObterLivrosViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int AnoDePublicacao { get; set; }
    public int QuantidadePaginas { get; set; }
    public DateTime DataCriacao { get; set; }
    public decimal NotaMedia { get; set; }

    public ObterLivrosViewModel(int id, string titulo, string? descricao, string iSBN, string autor, string editora, string genero, int anoDePublicacao, int quantidadePaginas, DateTime dataCriacao, decimal notaMedia)
    {
        Id = id;
        Titulo = titulo;
        Descricao = descricao;
        ISBN = iSBN;
        Autor = autor;
        Editora = editora;
        Genero = genero;
        AnoDePublicacao = anoDePublicacao;
        QuantidadePaginas = quantidadePaginas;
        DataCriacao = dataCriacao;
        NotaMedia = notaMedia;
    }
}
