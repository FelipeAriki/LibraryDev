using LibraryDev.Application.Commands.Livros;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Livros;
using Microsoft.AspNetCore.Mvc;

namespace LibraryDev.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivroController : ControllerBase
    {
        private readonly ILivroService _livroService;
        public LivroController(ILivroService livroService)
        {
            _livroService = livroService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterLivros()
        {
            var result = await _livroService.ObterLivrosAsync();
            if(result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterLivroPorId(int id)
        {
            var result = await _livroService.ObterLivroPorIdAsync(new ObterLivroPorIdQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CriarLivro([FromBody] CriarLivroCommand command)
        {
            var result = await _livroService.CriarLivroAsync(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarLivro([FromBody] AtualizarLivroCommand command)
        {
            var result = await _livroService.AtualizarLivroAsync(command);
            if (!result)
                return BadRequest("Livro não atualizado.");
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletarLivro(int id)
        {
            var result = await _livroService.DeletarLivroAsync(id);
            if (!result)
                return BadRequest("Livro não excluído.");
            return NoContent();
        }
    }
}
