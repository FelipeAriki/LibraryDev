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
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Avaliacao>> ObterAvaliacoesAsync()
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT a.Id, a.Nota, a.Descricao, a.IdUsuario, a.IdLivro,
                   a.DataInicioLeitura, a.DataFimLeitura, a.DataCriacao,
                   u.Id, u.Nome, u.Email,
                   l.Id, l.Titulo, l.Autor
            FROM Avaliacao a
            INNER JOIN Usuario u ON u.Id = a.IdUsuario
            INNER JOIN Livro   l ON l.Id = a.IdLivro
            ORDER BY a.DataCriacao DESC";

        return await QueryAvaliacaoComJoinsAsync(conn, sql, null);
    }

    public async Task<IEnumerable<Avaliacao>> ObterAvaliacoesPorLivroAsync(int idLivro)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT a.Id, a.Nota, a.Descricao, a.IdUsuario, a.IdLivro,
                   a.DataInicioLeitura, a.DataFimLeitura, a.DataCriacao,
                   u.Id, u.Nome, u.Email,
                   l.Id, l.Titulo, l.Autor
            FROM Avaliacao a
            INNER JOIN Usuario u ON u.Id = a.IdUsuario
            INNER JOIN Livro   l ON l.Id = a.IdLivro
            WHERE a.IdLivro = @IdLivro
            ORDER BY a.DataCriacao DESC";

        return await QueryAvaliacaoComJoinsAsync(conn, sql, new { IdLivro = idLivro });
    }

    public async Task<Avaliacao?> ObterAvaliacaoPorIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT a.Id, a.Nota, a.Descricao, a.IdUsuario, a.IdLivro,
                   a.DataInicioLeitura, a.DataFimLeitura, a.DataCriacao,
                   u.Id, u.Nome, u.Email,
                   l.Id, l.Titulo, l.Autor
            FROM Avaliacao a
            INNER JOIN Usuario u ON u.Id = a.IdUsuario
            INNER JOIN Livro   l ON l.Id = a.IdLivro
            WHERE a.Id = @Id";

        var results = await QueryAvaliacaoComJoinsAsync(conn, sql, new { Id = id });
        return results.FirstOrDefault();
    }

    public async Task<decimal> CalcularNotaMediaLivroAsync(int idLivro)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT ISNULL(AVG(CAST(Nota AS DECIMAL(5,2))), 0)
            FROM Avaliacao
            WHERE IdLivro = @IdLivro";
        return await conn.ExecuteScalarAsync<decimal>(sql, new { IdLivro = idLivro });
    }

    private static async Task<IEnumerable<Avaliacao>> QueryAvaliacaoComJoinsAsync(
        IDbConnection conn, string sql, object? param)
    {
        var result = new List<Avaliacao>();

        await conn.QueryAsync<Avaliacao, Usuario, Livro, Avaliacao>(
            sql,
            (avaliacao, usuario, livro) =>
            {
                avaliacao.Usuario = usuario;
                avaliacao.Livro = livro;
                result.Add(avaliacao);
                return avaliacao;
            },
            param,
            splitOn: "Id,Id");

        return result;
    }
}
