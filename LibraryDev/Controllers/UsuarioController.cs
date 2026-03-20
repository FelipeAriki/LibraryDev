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

        /// <summary>Lista todos os usuários.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterUsuarios()
        {
            var usuarios = await _usuarioService.ObterUsuariosAsync();
            return Ok(usuarios);
        }

        /// <summary>Retorna os detalhes de um usuário com suas avaliações.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioService.ObterUsuarioPorIdAsync(new ObterUsuarioPorIdQuery(id));
            if (usuario is null) return NotFound(new { mensagem = "Usuário não encontrado." });
            return Ok(usuario);
        }

        /// <summary>Cadastra um novo usuário.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioCommand command)
        {
            var (sucesso, mensagem, id) = await _usuarioService.CriarUsuarioAsync(command);
            if (!sucesso) return BadRequest(new { mensagem });
            return CreatedAtAction(nameof(ObterUsuarioPorId), new { id }, new { id, mensagem });
        }

        /// <summary>Atualiza os dados de um usuário.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] AtualizarUsuarioCommand command)
        {
            command.Id = id;
            var (sucesso, mensagem) = await _usuarioService.AtualizarUsuarioAsync(command);
            if (!sucesso)
            {
                if (mensagem.Contains("não encontrado")) return NotFound(new { mensagem });
                return BadRequest(new { mensagem });
            }
            return NoContent();
        }

        /// <summary>Remove um usuário pelo Id.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            var (sucesso, mensagem) = await _usuarioService.DeletarUsuarioAsync(id);
            if (!sucesso) return NotFound(new { mensagem });
            return NoContent();
        }
    }
}
