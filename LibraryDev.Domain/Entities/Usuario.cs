using System.Collections.Generic;

namespace LibraryDev.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiracao { get; set; }
        public string? TokenRecuperacaoSenha { get; set; }
        public DateTime? TokenRecuperacaoExpiracao { get; set; }
        public List<Avaliacao> Avaliacoes { get; set; } = [];
    }
}
