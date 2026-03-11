using LibraryDev.Application.Commands.Usuarios;
using LibraryDev.Application.Interfaces;
using LibraryDev.Application.Queries.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryDev.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuarios()
        {
            var usuarios = await _usuarioService.ObterUsuariosAsync();
            if (!usuarios.Any()) return NotFound();
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioService.ObterUsuarioPorIdAsync(new ObterUsuarioPorIdQuery(id));
            if (usuario is null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioCommand command)
        {
            var usuarioId = await _usuarioService.CriarUsuarioAsync(command);
            return CreatedAtAction(nameof(ObterUsuarioPorId), new { id = usuarioId }, null);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarUsuario([FromBody] AtualizarUsuarioCommand command)
        {
            var resultado = await _usuarioService.AtualizarUsuarioAsync(command);
            if (!resultado) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            var resultado = await _usuarioService.DeletarUsuarioAsync(id);
            if (!resultado) return NotFound();
            return NoContent();
        }
    }
}
