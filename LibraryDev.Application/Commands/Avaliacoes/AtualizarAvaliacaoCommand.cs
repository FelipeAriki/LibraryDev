using LibraryDev.Domain.Entities;

namespace LibraryDev.Application.Commands.Avaliacoes
{
    public class AtualizarAvaliacaoCommand
    {
        public int Id { get; set; }
        public int Nota { get; set; }
        public string? Descricao { get; set; }
        public int IdUsuario { get; set; }
        public int IdLivro { get; set; }
        public DateTime DataCriacao { get; set; }

        public AtualizarAvaliacaoCommand(int id, int nota, string? descricao, int idUsuario, int idLivro, DateTime dataCriacao)
        {
            Id = id;
            Nota = nota;
            Descricao = descricao;
            IdUsuario = idUsuario;
            IdLivro = idLivro;
            DataCriacao = dataCriacao;
        }

        public static Avaliacao ToEntity(AtualizarAvaliacaoCommand command)
        {
            return new Avaliacao
            {
                Id = command.Id,
                Nota = command.Nota,
                Descricao = command.Descricao,
                IdUsuario = command.IdUsuario,
                IdLivro = command.IdLivro,
                DataCriacao = command.DataCriacao
            };
        }
    }
}
