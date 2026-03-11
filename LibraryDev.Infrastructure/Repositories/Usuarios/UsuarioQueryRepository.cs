using Dapper;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Usuarios;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Usuarios;

public class UsuarioQueryRepository : IUsuarioQueryRepository
{
    private readonly string _connectionString;
    public UsuarioQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
    {
        using var conn = CreateConnection();
        var sql = "SELECT * FROM Usuario";
        return await conn.QueryAsync<Usuario>(sql);
    }

    public async Task<Usuario?> ObterUsuarioPorIdAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Usuario WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Usuario?>(sql, new { Id = id });
    }
}
