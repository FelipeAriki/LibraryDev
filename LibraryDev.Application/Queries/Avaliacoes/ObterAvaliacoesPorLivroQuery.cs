namespace LibraryDev.Application.Queries.Avaliacoes;

public class ObterAvaliacoesPorLivroQuery
{
    public int IdLivro { get; set; }

    public ObterAvaliacoesPorLivroQuery(int idLivro)
    {
        IdLivro = idLivro;
    }
}
