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
        var sql = "INSERT INTO Usuario (Email, Nome) VALUES (@Email, @Nome); SELECT CAST(SCOPE_IDENTITY() as int)";
        return await conn.QuerySingleAsync<int>(sql, new { usuario.Email, usuario.Nome });
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
}
