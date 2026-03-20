using Dapper;
using LibraryDev.Domain.Entities;
using LibraryDev.Domain.Interfaces.Livros;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryDev.Infrastructure.Repositories.Livros;

public class LivroCommandRepository : ILivroCommandRepository
{
    private readonly string _connectionString;

    public LivroCommandRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> CriarLivroAsync(Livro livro)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO Livro (Titulo, Descricao, ISBN, Autor, Editora, Genero,
                               AnoDePublicacao, QuantidadePaginas, CapaLivro)
            OUTPUT INSERTED.Id
            VALUES (@Titulo, @Descricao, @ISBN, @Autor, @Editora, @Genero,
                    @AnoDePublicacao, @QuantidadePaginas, @CapaLivro)";

        var parameters = new DynamicParameters();
        parameters.Add("Titulo", livro.Titulo);
        parameters.Add("Descricao", livro.Descricao);
        parameters.Add("ISBN", livro.ISBN);
        parameters.Add("Autor", livro.Autor);
        parameters.Add("Editora", livro.Editora);
        parameters.Add("Genero", (byte)livro.Genero);
        parameters.Add("AnoDePublicacao", livro.AnoDePublicacao);
        parameters.Add("QuantidadePaginas", livro.QuantidadePaginas);
        parameters.Add("CapaLivro", livro.CapaLivro, DbType.Binary);

        return await conn.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<bool> AtualizarLivroAsync(Livro livro)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE Livro SET
                Titulo           = @Titulo,
                Descricao        = @Descricao,
                ISBN             = @ISBN,
                Autor            = @Autor,
                Editora          = @Editora,
                Genero           = @Genero,
                AnoDePublicacao  = @AnoDePublicacao,
                QuantidadePaginas = @QuantidadePaginas
            WHERE Id = @Id";

        var rows = await conn.ExecuteAsync(sql, new
        {
            livro.Id,
            livro.Titulo,
            livro.Descricao,
            livro.ISBN,
            livro.Autor,
            livro.Editora,
            Genero = (byte)livro.Genero,
            livro.AnoDePublicacao,
            livro.QuantidadePaginas
        });
        return rows > 0;
    }

    public async Task<bool> DeletarLivroAsync(int id)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync("DELETE FROM Livro WHERE Id = @Id", new { Id = id });
        return rows > 0;
    }

    public async Task<bool> AtualizarCapaAsync(int id, byte[] capa)
    {
        using var conn = CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Id", id);
        parameters.Add("CapaLivro", capa, DbType.Binary);
        var rows = await conn.ExecuteAsync(
            "UPDATE Livro SET CapaLivro = @CapaLivro WHERE Id = @Id", parameters);
        return rows > 0;
    }

    public async Task<bool> AtualizarNotaMediaAsync(int id, decimal notaMedia)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "UPDATE Livro SET NotaMedia = @NotaMedia WHERE Id = @Id",
            new { Id = id, NotaMedia = notaMedia });
        return rows > 0;
    }
}
