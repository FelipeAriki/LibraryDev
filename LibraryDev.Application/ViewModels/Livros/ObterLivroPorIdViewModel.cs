using LibraryDev.Domain.Enums;

namespace LibraryDev.Application.ViewModels.Livros;

public class ObterLivroPorIdViewModel
{
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Editora { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int AnoDePublicacao { get; set; }
        public int QuantidadePaginas { get; set; }
        public DateTime DataCriacao { get; set; }
        public decimal NotaMedia { get; set; }
        public bool TemCapa { get; set; }
        public List<AvaliacaoResumidaViewModel> Avaliacoes { get; set; } = [];
}
