namespace LibraryDev.Application.ViewModels.Usuarios;

public class ObterUsuariosViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalAvaliacoes { get; set; }

    public ObterUsuariosViewModel(int id, string nome, string email, int totalAvaliacoes)
    {
        Id = id;
        Nome = nome;
        Email = email;
        TotalAvaliacoes = totalAvaliacoes;
    }
}
