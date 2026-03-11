using LibraryDev.Domain.Enums;

namespace LibraryDev.Application.ViewModels.Livros;

public class ObterLivroPorIdViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Autor { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public GeneroLivroEnum Genero { get; set; }

    public ObterLivroPorIdViewModel(string titulo, string? descricao, string autor, string editora, GeneroLivroEnum genero)
    {
        Titulo = titulo;
        Descricao = descricao;
        Autor = autor;
        Editora = editora;
        Genero = genero;
    }
}
