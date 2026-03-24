namespace LibraryDev.Domain.Services;

public interface IEmailService
{
    Task EnviarEmailRecuperacaoSenhaAsync(string email, string nome, string token);
}
