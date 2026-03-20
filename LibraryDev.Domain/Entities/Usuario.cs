using System.Collections.Generic;

namespace LibraryDev.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public List<Avaliacao> Avaliacoes { get; set; } = [];
    }
}
