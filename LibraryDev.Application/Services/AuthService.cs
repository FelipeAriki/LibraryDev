using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LibraryDev.Application.Commands.Auth;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Validators.Auth;
using LibraryDev.Application.ViewModels.Auth;
using LibraryDev.Domain.Interfaces.Usuarios;
using LibraryDev.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LibraryDev.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;
    private readonly IUsuarioCommandRepository _usuarioCommandRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioQueryRepository usuarioQueryRepository,
        IUsuarioCommandRepository usuarioCommandRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _usuarioQueryRepository = usuarioQueryRepository;
        _usuarioCommandRepository = usuarioCommandRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<(bool sucesso, string mensagem, LoginViewModel? resultado)> LoginAsync(LoginCommand command)
    {
        var (valido, mensagem) = AuthValidator.ValidarLogin(command);
        if (!valido) return (false, mensagem, null);

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorEmailAsync(command.Email);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(command.Senha, usuario.Senha))
            return (false, "E-mail ou senha inválidos.", null);

        var (accessToken, expiracao) = GerarAccessToken(usuario.Id, usuario.Email, usuario.Nome);
        var refreshToken = GerarRefreshToken();

        var refreshTokenExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var refreshTokenExpiracao = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        await _usuarioCommandRepository.AtualizarRefreshTokenAsync(usuario.Id, refreshToken, refreshTokenExpiracao);

        return (true, "Login realizado com sucesso.", new LoginViewModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiracaoUtc = expiracao
        });
    }

    public async Task<(bool sucesso, string mensagem, TokenRefreshViewModel? resultado)> RefreshTokenAsync(RefreshTokenCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return (false, "Refresh token é obrigatório.", null);

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorRefreshTokenAsync(command.RefreshToken);

        if (usuario is null || usuario.RefreshTokenExpiracao is null || usuario.RefreshTokenExpiracao < DateTime.UtcNow)
            return (false, "Refresh token inválido ou expirado.", null);

        var (accessToken, expiracao) = GerarAccessToken(usuario.Id, usuario.Email, usuario.Nome);
        var novoRefreshToken = GerarRefreshToken();

        var refreshTokenExpirationDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        var refreshTokenExpiracao = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

        await _usuarioCommandRepository.AtualizarRefreshTokenAsync(usuario.Id, novoRefreshToken, refreshTokenExpiracao);

        return (true, "Token renovado com sucesso.", new TokenRefreshViewModel
        {
            AccessToken = accessToken,
            RefreshToken = novoRefreshToken,
            ExpiracaoUtc = expiracao
        });
    }

    public async Task<(bool sucesso, string mensagem)> RecuperarSenhaAsync(RecuperarSenhaCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return (true, "Se o e-mail estiver cadastrado, você receberá as instruções de recuperação.");

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorEmailAsync(command.Email);

        if (usuario is not null)
        {
            var token = GerarTokenRecuperacao();
            var expiracao = DateTime.UtcNow.AddMinutes(30);

            await _usuarioCommandRepository.AtualizarTokenRecuperacaoAsync(usuario.Id, token, expiracao);
            await _emailService.EnviarEmailRecuperacaoSenhaAsync(usuario.Email, usuario.Nome, token);
        }

        // Sempre retorna sucesso para não revelar se o email existe
        return (true, "Se o e-mail estiver cadastrado, você receberá as instruções de recuperação.");
    }

    public async Task<(bool sucesso, string mensagem)> RedefinirSenhaAsync(RedefinirSenhaCommand command)
    {
        var (valido, mensagem) = AuthValidator.ValidarRedefinirSenha(command);
        if (!valido) return (false, mensagem);

        var usuario = await _usuarioQueryRepository.ObterUsuarioPorTokenRecuperacaoAsync(command.Token);

        if (usuario is null || usuario.TokenRecuperacaoExpiracao is null || usuario.TokenRecuperacaoExpiracao < DateTime.UtcNow)
            return (false, "Token inválido ou expirado.");

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(command.NovaSenha);
        await _usuarioCommandRepository.AtualizarSenhaAsync(usuario.Id, senhaHash);

        return (true, "Senha redefinida com sucesso.");
    }

    private (string token, DateTime expiracao) GerarAccessToken(int userId, string email, string nome)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret não configurado.");
        var issuer = _configuration["JwtSettings:Issuer"] ?? "LibraryDev";
        var audience = _configuration["JwtSettings:Audience"] ?? "LibraryDev";
        var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiracao = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiracao);
    }

    private static string GerarRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string GerarTokenRecuperacao()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "");
    }
}
