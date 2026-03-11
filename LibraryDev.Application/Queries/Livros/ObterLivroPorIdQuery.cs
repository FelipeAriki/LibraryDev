namespace LibraryDev.Application.Queries.Livros;

public class ObterLivroPorIdQuery
{
    public int Id { get; set; }

    public ObterLivroPorIdQuery(int id)
    {
        Id = id;
    }
}
