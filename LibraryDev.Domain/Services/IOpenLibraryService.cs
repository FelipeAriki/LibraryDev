namespace LibraryDev.Domain.Services;

public interface IOpenLibraryService
{
    Task<LivroExternoViewModel?> BuscarPorISBNAsync(string isbn);
}

public class LivroExternoViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string? Editora { get; set; }
    public string? ISBN { get; set; }
    public int? AnoDePublicacao { get; set; }
    public string? Descricao { get; set; }
    public string? CapaUrl { get; set; }
}