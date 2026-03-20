namespace LibraryDev.Application.ViewModels.Relatorios;

public class RelatorioLivrosLidosViewModel
{
    public int Ano { get; set; }
    public int TotalLivrosLidos { get; set; }
    public List<LivroLidoItemViewModel> Livros { get; set; } = [];
}

public class LivroLidoItemViewModel
{
    public string TituloLivro { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public int Nota { get; set; }
    public DateTime DataInicioLeitura { get; set; }
    public DateTime DataFimLeitura { get; set; }
    public int DiasLeitura { get; set; }
}
