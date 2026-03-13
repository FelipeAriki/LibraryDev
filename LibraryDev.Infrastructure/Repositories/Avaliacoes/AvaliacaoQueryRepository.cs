using Dapper;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Avaliacoes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Avaliacoes;

public class AvaliacaoQueryRepository : IAvaliacaoQueryRepository
{
    private readonly string _connectionString;
    public AvaliacaoQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Avaliacao>> ObterAvaliacoesAsync()
    {
        using var conn = CreateConnection();
        var sql = "SELECT * FROM Avaliacao";
        return await conn.QueryAsync<Avaliacao>(sql);
    }

    public async Task<Avaliacao?> ObterAvaliacaoPorIdAsync(int id)
    {
       using var conn = CreateConnection();
       var sql = "SELECT * FROM Avaliacao WHERE Id = @id";
        return await conn.QueryFirstOrDefaultAsync<Avaliacao?>(sql, new { id });
    }
}
