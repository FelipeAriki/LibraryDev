namespace LibraryDev.Application.ViewModels.Avaliacoes;

public class ObterAvaliacoesViewModel
{
    public int Id { get; set; }
    public int Nota { get; set; }
    public string? Descricao { get; set; }
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public DateTime DataCriacao { get; set; }

    public ObterAvaliacoesViewModel(int id, int nota, string? descricao, int idUsuario, int idLivro, DateTime dataCriacao)
    {
        Id = id;
        Nota = nota;
        Descricao = descricao;
        IdUsuario = idUsuario;
        IdLivro = idLivro;
        DataCriacao = dataCriacao;
    }
}
