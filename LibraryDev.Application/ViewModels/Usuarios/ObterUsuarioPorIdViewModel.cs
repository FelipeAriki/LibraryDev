namespace LibraryDev.Application.ViewModels.Usuarios;

public class ObterUsuarioPorIdViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }

    public ObterUsuarioPorIdViewModel(int id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
    }
}
