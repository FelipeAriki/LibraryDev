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

        [HttpGet]
        public async Task<IActionResult> ObterAvaliacoes()
        {
            var resultado = await _avaliacaoService.ObterAvaliacoes();
            if (!resultado.Any()) return BadRequest("Nenhuma avaliação encontrada");
            return Ok(resultado);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterAvaliacaoPorId(int id)
        {
            var resultado = await _avaliacaoService.ObterAvalicaoPorId(new ObterAvaliacaoPorIdQuery(id));
            if (resultado is null) return BadRequest("Nenhuma avaliação encontrada");
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CriarAvaliacao([FromBody] CriarAvaliacaoCommand command)
        {
            var resultado = await _avaliacaoService.CriarAvaliacaoAsync(command);
            if (resultado == 0) return BadRequest("Não foi possível criar a avaliação.");
            return Ok("Avaliação criada com sucesso: " + resultado);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarAvaliacao([FromBody] AtualizarAvaliacaoCommand command)
        {
            var resultado = await _avaliacaoService.AtualizarAvaliacaoAsync(command);
            if (!resultado) return BadRequest("Não foi possível atualizar a avaliação.");
            return Ok("Avaliação atualizada com sucesso.");
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletarAvaliacao(int id)
        {
            var resultado = await _avaliacaoService.DeletarAvaliacaoAsync(id);
            if (!resultado) return BadRequest("Não foi possível deletar a avaliação.");
            return Ok("Avaliação deletada com sucesso.");
        }
    }
}
