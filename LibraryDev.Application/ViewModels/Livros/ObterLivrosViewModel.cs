using LibraryDev.Domain.Enums;

namespace LibraryDev.Application.ViewModels.Livros;

public class ObterLivrosViewModel
{
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

    public ObterLivrosViewModel(string titulo, string? descricao, string iSBN, string autor, string editora, GeneroLivroEnum genero, int anoDePublicacao, int quantidadePaginas, DateTime dataCriacao, decimal notaMedia, byte[] capaLivro)
    {
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
        CapaLivro = capaLivro;
    }
}
