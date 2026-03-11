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
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<int> CriarLivroAsync(Livro livro)
    {
        using var conn = CreateConnection();
        var sql = @"INSERT INTO Livro(titulo, descricao, isbn, autor, editora, genero, anoDePublicacao, quantidadePaginas, dataCriacao, notaMedia, capaLivro)
                    OUTPUT INSERTED.Id
                    VALUES(@titulo, @descricao, @isbn, @autor, @editora, @genero, @anoDePublicacao, @quantidadePaginas, @dataCriacao, @notaMedia, @capaLivro)";

        var parameters = new DynamicParameters();
        parameters.Add("titulo", livro.Titulo);
        parameters.Add("descricao", livro.Descricao);
        parameters.Add("isbn", livro.ISBN);
        parameters.Add("autor", livro.Autor);
        parameters.Add("editora", livro.Editora);
        parameters.Add("genero", livro.Genero);
        parameters.Add("anoDePublicacao", livro.AnoDePublicacao);
        parameters.Add("quantidadePaginas", livro.QuantidadePaginas);
        parameters.Add("dataCriacao", livro.DataCriacao);
        parameters.Add("notaMedia", livro.NotaMedia);
        parameters.Add("capaLivro", livro.CapaLivro, DbType.Binary);

        return await conn.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<bool> AtualizarLivroAsync(Livro livro)
    {
        using var conn = CreateConnection();
        var sql = @"UPDATE Livro SET 
                    titulo = @titulo, 
                    descricao = @descricao, 
                    isbn = @isbn, 
                    autor = @autor, 
                    editora = @editora, 
                    genero = @genero, 
                    anoDePublicacao = @anoDePublicacao, 
                    quantidadePaginas = @quantidadePaginas, 
                    dataCriacao = @dataCriacao, 
                    notaMedia = @notaMedia, 
                    capaLivro = @capaLivro
                    WHERE Id = @Id";
        return await conn.ExecuteAsync(sql, new
        {
            livro.Id,
            titulo = livro.Titulo,
            descricao = livro.Descricao,
            isbn = livro.ISBN,
            autor = livro.Autor,
            editora = livro.Editora,
            genero = livro.Genero,
            anoDePublicacao = livro.AnoDePublicacao,
            quantidadePaginas = livro.QuantidadePaginas,
            dataCriacao = livro.DataCriacao,
            notaMedia = livro.NotaMedia,
            capaLivro = livro.CapaLivro
        }).ContinueWith(t => t.Result > 0);
    }

    public async Task<bool> DeletarLivroAsync(int id)
    {
        using var conn = CreateConnection();
        var sql = "DELETE FROM Livro WHERE Id = @Id";
        return await conn.ExecuteAsync(sql, new { Id = id }).ContinueWith(t => t.Result > 0);
    }
}
