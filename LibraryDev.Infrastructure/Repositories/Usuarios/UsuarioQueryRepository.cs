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
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT u.Id, u.Nome, u.Email,
                   COUNT(a.Id) AS TotalAvaliacoes
            FROM Usuario u
            LEFT JOIN Avaliacao a ON a.IdUsuario = u.Id
            GROUP BY u.Id, u.Nome, u.Email
            ORDER BY u.Nome";

        return await conn.QueryAsync<Usuario>(sql);
    }

    public async Task<Usuario?> ObterUsuarioPorIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = "SELECT Id, Nome, Email FROM Usuario WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }

    public async Task<Usuario?> ObterUsuarioComAvaliacoesPorIdAsync(int id)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT u.Id, u.Nome, u.Email,
                   a.Id, a.Nota, a.Descricao, a.IdUsuario, a.IdLivro,
                   a.DataInicioLeitura, a.DataFimLeitura, a.DataCriacao,
                   l.Id, l.Titulo, l.Autor
            FROM Usuario u
            LEFT JOIN Avaliacao a ON a.IdUsuario = u.Id
            LEFT JOIN Livro l ON l.Id = a.IdLivro
            WHERE u.Id = @Id";

        var usuarioDictionary = new Dictionary<int, Usuario>();

        await conn.QueryAsync<Usuario, Avaliacao, Livro, Usuario>(
            sql,
            (usuario, avaliacao, livro) =>
            {
                if (!usuarioDictionary.TryGetValue(usuario.Id, out var u))
                {
                    u = usuario;
                    usuarioDictionary.Add(u.Id, u);
                }

                if (avaliacao?.Id > 0)
                {
                    if (livro?.Id > 0)
                        avaliacao.Livro = livro;
                    u.Avaliacoes.Add(avaliacao);
                }

                return u;
            },
            new { Id = id },
            splitOn: "Id,Id");

        return usuarioDictionary.Values.FirstOrDefault();
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, int? ignorarId = null)
    {
        using var conn = CreateConnection();
        const string sql = @"
            SELECT COUNT(1) FROM Usuario
            WHERE Email = @Email AND (@IgnorarId IS NULL OR Id <> @IgnorarId)";
        return await conn.ExecuteScalarAsync<int>(sql, new { Email = email, IgnorarId = ignorarId }) > 0;
    }
}
