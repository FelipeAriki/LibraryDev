using LibraryDev.Domain.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace LibraryDev.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task EnviarEmailRecuperacaoSenhaAsync(string email, string nome, string token)
    {
        var smtpHost = _configuration["SmtpSettings:Host"] ?? throw new InvalidOperationException("SmtpSettings:Host não configurado.");
        var smtpPort = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
        var smtpEmail = _configuration["SmtpSettings:Email"] ?? throw new InvalidOperationException("SmtpSettings:Email não configurado.");
        var smtpPassword = _configuration["SmtpSettings:Password"] ?? throw new InvalidOperationException("SmtpSettings:Password não configurado.");
        var useSsl = bool.Parse(_configuration["SmtpSettings:UseSsl"] ?? "true");
        var urlRecuperacao = _configuration["AppSettings:UrlRecuperacaoSenha"] ?? "https://localhost:7054";

        var linkRecuperacao = $"{urlRecuperacao}?token={Uri.EscapeDataString(token)}";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("LibraryDev", smtpEmail));
        message.To.Add(new MailboxAddress(nome, email));
        message.Subject = "Recuperação de Senha - LibraryDev";

        var nomeSeguro = System.Net.WebUtility.HtmlEncode(nome);

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"""
                <h2>Olá, {nomeSeguro}!</h2>
                <p>Recebemos uma solicitação para redefinir sua senha.</p>
                <p>Use o link abaixo para criar uma nova senha. Este link expira em 30 minutos.</p>
                <p><a href="{linkRecuperacao}">Redefinir minha senha</a></p>
                <p>Se você não solicitou a recuperação de senha, ignore este e-mail.</p>
                <br/>
                <p>Atenciosamente,<br/>Equipe LibraryDev</p>
                """
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, useSsl);
        await client.AuthenticateAsync(smtpEmail, smtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
