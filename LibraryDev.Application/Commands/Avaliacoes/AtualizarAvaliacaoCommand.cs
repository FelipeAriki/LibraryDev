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
        public DateTime DataInicioLeitura { get; set; }
        public DateTime DataFimLeitura { get; set; }
    }
}
