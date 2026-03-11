namespace LibraryDev.Application.ViewModels.Usuarios;

public class ObterUsuariosViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }

    public ObterUsuariosViewModel(int id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
    }
}
