namespace LibraryDev.Application.ViewModels.Avaliacoes;

public class ObterAvaliacaoPorIdViewModel
{
    public int Id { get; set; }
    public int Nota { get; set; }
    public string? Descricao { get; set; }
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public int IdLivro { get; set; }
    public string TituloLivro { get; set; } = string.Empty;
    public DateTime DataInicioLeitura { get; set; }
    public DateTime DataFimLeitura { get; set; }
    public DateTime DataCriacao { get; set; }

    public ObterAvaliacaoPorIdViewModel(int id, int nota, string? descricao, int idUsuario, string nomeUsuario, int idLivro, string tituloLivro, DateTime dataInicioLeitura, DateTime dataFimLeitura, DateTime dataCriacao)
    {
        Id = id;
        Nota = nota;
        Descricao = descricao;
        IdUsuario = idUsuario;
        NomeUsuario = nomeUsuario;
        IdLivro = idLivro;
        TituloLivro = tituloLivro;
        DataInicioLeitura = dataInicioLeitura;
        DataFimLeitura = dataFimLeitura;
        DataCriacao = dataCriacao;
    }
}
