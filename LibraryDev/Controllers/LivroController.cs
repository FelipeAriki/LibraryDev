using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Livros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryDev.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LivroController : ControllerBase
    {
        private readonly ILivroService _livroService;

        public LivroController(ILivroService livroService)
        {
            _livroService = livroService;
        }

        /// <summary>Lista todos os livros cadastrados.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterLivros()
        {
            var livros = await _livroService.ObterLivrosAsync();
            return Ok(livros);
        }

        /// <summary>Retorna os detalhes de um livro pelo Id, incluindo suas avaliações.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterLivroPorId(int id)
        {
            var livro = await _livroService.ObterLivroPorIdAsync(new ObterLivroPorIdQuery(id));
            if (livro is null) return NotFound(new { mensagem = "Livro não encontrado." });
            return Ok(livro);
        }

        /// <summary>Cadastra um novo livro.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarLivro([FromBody] CriarLivroCommand command)
        {
            var (sucesso, mensagem, id) = await _livroService.CriarLivroAsync(command);
            if (!sucesso) return BadRequest(new { mensagem });
            return CreatedAtAction(nameof(ObterLivroPorId), new { id }, new { id, mensagem });
        }

        /// <summary>Atualiza os dados de um livro existente.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarLivro(int id, [FromBody] AtualizarLivroCommand command)
        {
            command.Id = id;
            var (sucesso, mensagem) = await _livroService.AtualizarLivroAsync(command);
            if (!sucesso)
            {
                if (mensagem.Contains("não encontrado")) return NotFound(new { mensagem });
                return BadRequest(new { mensagem });
            }
            return NoContent();
        }

        /// <summary>Remove um livro pelo Id.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarLivro(int id)
        {
            var (sucesso, mensagem) = await _livroService.DeletarLivroAsync(id);
            if (!sucesso) return NotFound(new { mensagem });
            return NoContent();
        }

        /// <summary>Faz upload da imagem de capa de um livro (PLUS).</summary>
        [HttpPost("{id:int}/capa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadCapa(int id, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest(new { mensagem = "Arquivo inválido." });

            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!extensoesPermitidas.Contains(extensao))
                return BadRequest(new { mensagem = "Formato de imagem não suportado. Use jpg, jpeg, png ou webp." });

            if (arquivo.Length > 5 * 1024 * 1024)
                return BadRequest(new { mensagem = "A imagem não pode ultrapassar 5 MB." });

            using var ms = new MemoryStream();
            await arquivo.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var (sucesso, mensagem) = await _livroService.UploadCapaAsync(id, bytes);
            if (!sucesso)
            {
                if (mensagem.Contains("não encontrado")) return NotFound(new { mensagem });
                return BadRequest(new { mensagem });
            }
            return Ok(new { mensagem });
        }

        /// <summary>Retorna a imagem de capa de um livro (PLUS).</summary>
        [HttpGet("{id:int}/capa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterCapa(int id)
        {
            var capa = await _livroService.ObterCapaAsync(id);
            if (capa == null || capa.Length == 0)
                return NotFound(new { mensagem = "Capa não encontrada para este livro." });
            return File(capa, "image/jpeg");
        }

        /// <summary>Consulta informações de um livro em uma API externa pelo ISBN (PLUS).</summary>
        [HttpGet("externo/{isbn}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConsultarLivroExterno(string isbn)
        {
            var livro = await _livroService.ConsultarLivroExternoAsync(isbn);
            if (livro is null) return NotFound(new { mensagem = "Livro não encontrado na API externa." });
            return Ok(livro);
        }
    }
}
