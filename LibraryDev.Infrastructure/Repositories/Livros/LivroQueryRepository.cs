using Dapper;
using LibraryDev.Application.Interfaces.Livros;
using LibraryDev.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Livros;

public class LivroQueryRepository : ILivroQueryRepository
{
    private readonly string _connectionString;
    public LivroQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    public async Task<IEnumerable<Livro>> ObterLivrosAsync()
    {
        using var conn = CreateConnection();
        var sql = "SELECT * FROM Livro";
        return await conn.QueryAsync<Livro>(sql);
    }

    public async Task<Livro?> ObterLivroPorIdAsync(int id)
    {
        using var conn = CreateConnection();
        var sql = "SELECT * FROM Livro WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Livro>(sql, new { Id = id });
    }
}
