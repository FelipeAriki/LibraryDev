namespace LibraryDev.Application.Queries.Usuarios;

public class ObterUsuarioPorIdQuery
{
    public int Id { get; set; }

    public ObterUsuarioPorIdQuery(int id)
    {
        Id = id;
    }
}
