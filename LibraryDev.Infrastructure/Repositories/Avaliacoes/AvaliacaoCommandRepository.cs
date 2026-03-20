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
        const string sql = @"
                                DECLARE @IdGerado TABLE (Id INT);

                                INSERT INTO Avaliacao (Nota, Descricao, IdUsuario, IdLivro,
                                                       DataInicioLeitura, DataFimLeitura)
                                OUTPUT INSERTED.Id INTO @IdGerado
                                VALUES (@Nota, @Descricao, @IdUsuario, @IdLivro,
                                        @DataInicioLeitura, @DataFimLeitura);

                                SELECT Id FROM @IdGerado;";

        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            avaliacao.Nota,
            avaliacao.Descricao,
            avaliacao.IdUsuario,
            avaliacao.IdLivro,
            avaliacao.DataInicioLeitura,
            avaliacao.DataFimLeitura
        });
    }

    public async Task<bool> AtualizarAvaliacaoAsync(Avaliacao avaliacao)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE Avaliacao SET
                Nota              = @Nota,
                Descricao         = @Descricao,
                IdUsuario         = @IdUsuario,
                IdLivro           = @IdLivro,
                DataInicioLeitura = @DataInicioLeitura,
                DataFimLeitura    = @DataFimLeitura
            WHERE Id = @Id";

        var rows = await conn.ExecuteAsync(sql, new
        {
            avaliacao.Nota,
            avaliacao.Descricao,
            avaliacao.IdUsuario,
            avaliacao.IdLivro,
            avaliacao.DataInicioLeitura,
            avaliacao.DataFimLeitura,
            avaliacao.Id
        });
        return rows > 0;
    }

    public async Task<bool> DeletarAvaliacaoAsync(int id)
    {
        using var conn = CreateConnection();
        var sql = "DELETE FROM Avaliacao WHERE Id = @Id";
        return await conn.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
