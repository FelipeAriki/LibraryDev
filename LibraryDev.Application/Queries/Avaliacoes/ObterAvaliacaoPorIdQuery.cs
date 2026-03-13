namespace LibraryDev.Application.Queries.Avaliacoes;

public class ObterAvaliacaoPorIdQuery
{
    public int Id { get; set; }

    public ObterAvaliacaoPorIdQuery(int id)
    {
        Id = id;
    }
}
