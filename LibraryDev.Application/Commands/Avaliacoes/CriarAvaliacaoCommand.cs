using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Commands.Avaliacoes;

public class CriarAvaliacaoCommand
{
    public int Nota { get; set; }
    public string? Descricao { get; set; }
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public DateTime DataCriacao { get; set; }

    public CriarAvaliacaoCommand(int nota, string? descricao, int idUsuario, int idLivro, DateTime dataCriacao)
    {
        Nota = nota;
        Descricao = descricao;
        IdUsuario = idUsuario;
        IdLivro = idLivro;
        DataCriacao = dataCriacao;
    }

    public static Avaliacao ToEntity(CriarAvaliacaoCommand command)
    {
        return new Avaliacao
        {
            Nota = command.Nota,
            Descricao = command.Descricao,
            IdUsuario = command.IdUsuario,
            IdLivro = command.IdLivro,
            DataCriacao = command.DataCriacao
        };
    }
}
