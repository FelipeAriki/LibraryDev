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
        const string sql = @"
            SELECT Id, Titulo, Descricao, ISBN, Autor, Editora, Genero,
                   AnoDePublicacao, QuantidadePaginas, DataCriacao, NotaMedia, CapaLivro
            FROM Livro
            ORDER BY Titulo";
        return await conn.QueryAsync<Livro>(sql);
    }

    public async Task<Livro?> ObterLivroPorIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
        SELECT l.Id, l.Titulo, l.Descricao, l.ISBN, l.Autor, l.Editora, l.Genero,
               l.AnoDePublicacao, l.QuantidadePaginas, l.DataCriacao, l.NotaMedia, l.CapaLivro,
               a.Id, a.Nota, a.Descricao, a.IdUsuario, a.IdLivro,
               a.DataInicioLeitura, a.DataFimLeitura, a.DataCriacao,
               u.Id, u.Nome, u.Email
        FROM Livro l
        LEFT JOIN Avaliacao a ON a.IdLivro = l.Id
        LEFT JOIN Usuario u ON u.Id = a.IdUsuario
        WHERE l.Id = @Id";

        var livroDictionary = new Dictionary<int, Livro>();

        await conn.QueryAsync<Livro, Avaliacao, Usuario, Livro>(
            sql,
            (livro, avaliacao, usuario) =>
            {
                if (!livroDictionary.TryGetValue(livro.Id, out var livroExistente))
                {
                    livroExistente = livro;
                    livroDictionary.Add(livroExistente.Id, livroExistente);
                }

                if (avaliacao?.Id > 0)
                {
                    avaliacao.Usuario = usuario?.Id > 0 ? usuario : null;
                    livroExistente.Avaliacoes.Add(avaliacao);
                }

                return livroExistente;
            },
            new { Id = id },
            splitOn: "Id,Id");

        return livroDictionary.Values.FirstOrDefault();
    }

    public async Task<Livro?> ObterLivroPorISBNAsync(string isbn)
    {
        using var conn = CreateConnection();
        const string sql = "SELECT Id, Titulo, ISBN FROM Livro WHERE ISBN = @ISBN";
        return await conn.QueryFirstOrDefaultAsync<Livro>(sql, new { ISBN = isbn });
    }
}
