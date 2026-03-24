using Dapper;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Usuarios;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Usuarios;

public class UsuarioCommandRepository : IUsuarioCommandRepository
{
    private readonly string _connectionString;
    public UsuarioCommandRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> CriarUsuarioAsync(Usuario usuario)
    {
        using var conn = CreateConnection();
        var sql = "INSERT INTO Usuario (Email, Nome, Senha) VALUES (@Email, @Nome, @Senha); SELECT CAST(SCOPE_IDENTITY() as int)";
        return await conn.QuerySingleAsync<int>(sql, new { usuario.Email, usuario.Nome, usuario.Senha });
    }

    public async Task<bool> AtualizarUsuarioAsync(Usuario usuario)
    {
        using var conn = CreateConnection();
        var sql = "UPDATE Usuario SET Email = @Email, Nome = @Nome WHERE Id = @Id";
        var rowsAffected = await conn.ExecuteAsync(sql, new { usuario.Email, usuario.Nome, usuario.Id });
        return rowsAffected > 0;
    }

    public async Task<bool> DeletarUsuarioAsync(int id)
    {
        using var conn = CreateConnection();
        var sql = "DELETE FROM Usuario WHERE Id = @Id";
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> AtualizarRefreshTokenAsync(int id, string? refreshToken, DateTime? expiracao)
    {
        using var conn = CreateConnection();
        var sql = "UPDATE Usuario SET RefreshToken = @RefreshToken, RefreshTokenExpiracao = @Expiracao WHERE Id = @Id";
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, RefreshToken = refreshToken, Expiracao = expiracao });
        return rowsAffected > 0;
    }

    public async Task<bool> AtualizarTokenRecuperacaoAsync(int id, string? token, DateTime? expiracao)
    {
        using var conn = CreateConnection();
        var sql = "UPDATE Usuario SET TokenRecuperacaoSenha = @Token, TokenRecuperacaoExpiracao = @Expiracao WHERE Id = @Id";
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, Token = token, Expiracao = expiracao });
        return rowsAffected > 0;
    }

    public async Task<bool> AtualizarSenhaAsync(int id, string senhaHash)
    {
        using var conn = CreateConnection();
        var sql = "UPDATE Usuario SET Senha = @Senha, TokenRecuperacaoSenha = NULL, TokenRecuperacaoExpiracao = NULL WHERE Id = @Id";
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, Senha = senhaHash });
        return rowsAffected > 0;
    }
}
