using Dapper;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Avaliacoes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Avaliacoes;

public class AvaliacaoCommandRepository : IAvaliacaoCommandRepository
{
    private readonly string _connectionString;
    public AvaliacaoCommandRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> CriarAvaliacaoAsync(Avaliacao avaliacao)
    {
        using var conn = CreateConnection();
        var sql = @"
                    INSERT INTO Avaliacao
                    (
                        Nota,
                        Descricao,
                        IdUsuario,
                        IdLivro,
                        DataCriacao
                    )
                    OUTPUT INSERTED.Id
                    VALUES
                    (
                        @Nota,
                        @Descricao,
                        @IdUsuario,
                        @IdLivro,
                        @DataCriacao
                    )";

        return await conn.QueryFirstOrDefaultAsync<int>(sql, new
        {
            avaliacao.Nota,
            avaliacao.Descricao,
            avaliacao.IdUsuario,
            avaliacao.IdLivro,
            DataCriacao = DateTime.UtcNow
        });
    }

    public async Task<bool> AtualizarAvaliacaoAsync(Avaliacao avaliacao)
    {
        using var conn = CreateConnection();
        var sql = @"
                    UPDATE Avaliacao
                    SET
                        Nota = @Nota,
                        Descricao = @Descricao,
                        IdUsuario = @IdUsuario,
                        IdLivro = @IdLivro
                    WHERE Id = @Id";
        return await conn.ExecuteAsync(sql, new
        {
            avaliacao.Nota,
            avaliacao.Descricao,
            avaliacao.IdUsuario,
            avaliacao.IdLivro,
            avaliacao.Id
        }) > 0;
    }

    public async Task<bool> DeletarAvaliacaoAsync(int id)
    {
        using var conn = CreateConnection();
        var sql = "DELETE FROM Avaliacao WHERE Id = @Id";
        return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
