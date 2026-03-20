namespace LibraryDev.Application.ViewModels.Usuarios;

public class ObterUsuarioPorIdViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<AvaliacaoDoUsuarioViewModel> Avaliacoes { get; set; } = [];
}
