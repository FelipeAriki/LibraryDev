namespace LibraryDev.Application.ViewModels.Usuarios;

public class AvaliacaoDoUsuarioViewModel
{
    public int Id { get; set; }
    public int Nota { get; set; }
    public string? Descricao { get; set; }
    public string TituloLivro { get; set; } = string.Empty;
    public DateTime DataInicioLeitura { get; set; }
    public DateTime DataFimLeitura { get; set; }
    public DateTime DataCriacao { get; set; }
}
