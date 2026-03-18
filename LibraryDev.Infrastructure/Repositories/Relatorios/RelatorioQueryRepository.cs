using Dapper;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Relatorios;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Relatorios;

public class RelatorioQueryRepository : IRelatorioQueryRepository
{
    private readonly string _connectionString;
    public RelatorioQueryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Avaliacao>> ObterLivrosLidosPorAnoAsync(int ano)
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
            WHERE YEAR(a.DataFimLeitura) = @Ano
            ORDER BY a.DataFimLeitura";

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
            new { Ano = ano },
            splitOn: "Id,Id");

        return result;
    }
}
