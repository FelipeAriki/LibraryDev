namespace LibraryDev.Application.ViewModels.Auth;

public class LoginViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiracaoUtc { get; set; }
}
