using LibraryDev.Application.Commands.Avaliacoes;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Avaliacoes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryDev.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvaliacaoController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;
        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        /// <summary>Lista todas as avaliações.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterAvaliacoes()
        {
            var avaliacoes = await _avaliacaoService.ObterAvaliacoesAsync();
            return Ok(avaliacoes);
        }

        /// <summary>Lista as avaliações de um livro específico.</summary>
        [HttpGet("livro/{idLivro:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterAvaliacoesPorLivro(int idLivro)
        {
            var avaliacoes = await _avaliacaoService.ObterAvaliacoesPorLivroAsync(new ObterAvaliacoesPorLivroQuery(idLivro));
            return Ok(avaliacoes);
        }

        /// <summary>Retorna os detalhes de uma avaliação pelo Id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterAvaliacaoPorId(int id)
        {
            var avaliacao = await _avaliacaoService.ObterAvaliacaoPorIdAsync(new ObterAvaliacaoPorIdQuery(id));
            if (avaliacao is null) return NotFound(new { mensagem = "Avaliação não encontrada." });
            return Ok(avaliacao);
        }

        /// <summary>Cadastra uma nova avaliação para um livro.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarAvaliacao([FromBody] CriarAvaliacaoCommand command)
        {
            var (sucesso, mensagem, id) = await _avaliacaoService.CriarAvaliacaoAsync(command);
            if (!sucesso) return BadRequest(new { mensagem });
            return CreatedAtAction(nameof(ObterAvaliacaoPorId), new { id }, new { id, mensagem });
        }

        /// <summary>Atualiza uma avaliação existente.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarAvaliacao(int id, [FromBody] AtualizarAvaliacaoCommand command)
        {
            command.Id = id;
            var (sucesso, mensagem) = await _avaliacaoService.AtualizarAvaliacaoAsync(command);
            if (!sucesso)
            {
                if (mensagem.Contains("não encontrada")) return NotFound(new { mensagem });
                return BadRequest(new { mensagem });
            }
            return NoContent();
        }

        /// <summary>Remove uma avaliação pelo Id.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarAvaliacao(int id)
        {
            var (sucesso, mensagem) = await _avaliacaoService.DeletarAvaliacaoAsync(id);
            if (!sucesso) return NotFound(new { mensagem });
            return NoContent();
        }
    }
}
