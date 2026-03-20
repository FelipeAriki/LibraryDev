using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Commands.Avaliacoes;

public class CriarAvaliacaoCommand
{
    public int Nota { get; set; }
    public string? Descricao { get; set; }
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public DateTime DataInicioLeitura { get; set; }
    public DateTime DataFimLeitura { get; set; }

    public static Avaliacao ToEntity(CriarAvaliacaoCommand command) => new()
    {
        Nota = command.Nota,
        Descricao = command.Descricao,
        IdUsuario = command.IdUsuario,
        IdLivro = command.IdLivro,
        DataInicioLeitura = command.DataInicioLeitura,
        DataFimLeitura = command.DataFimLeitura,
        DataCriacao = DateTime.UtcNow
    };
}
