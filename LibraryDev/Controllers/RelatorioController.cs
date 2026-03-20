using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Relatorios;
using Microsoft.AspNetCore.Mvc;

namespace LibraryDev.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatorioController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        /// <summary>
        /// Gera um relatório com os livros lidos em um determinado ano (PLUS).
        /// Filtra pelas avaliações cuja data de fim de leitura pertence ao ano informado.
        /// </summary>
        [HttpGet("livros-lidos/{ano:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterRelatorioLivrosLidos(int ano)
        {
            if (ano < 1900 || ano > DateTime.UtcNow.Year)
                return BadRequest(new { mensagem = $"Ano inválido. Informe um valor entre 1900 e {DateTime.UtcNow.Year}." });

            var relatorio = await _relatorioService.ObterRelatorioLivrosLidosAsync(
                new ObterRelatorioLivrosLidosQuery(ano));
            return Ok(relatorio);
        }
    }
}
