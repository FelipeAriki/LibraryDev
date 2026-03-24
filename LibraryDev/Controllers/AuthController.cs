using LibraryDev.Application.Commands.Auth;
using LibraryDev.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryDev.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Realiza login e retorna o token JWT.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var (sucesso, mensagem, resultado) = await _authService.LoginAsync(command);
            if (!sucesso) return Unauthorized(new { mensagem });
            return Ok(resultado);
        }

        /// <summary>Renova o token JWT usando um refresh token.</summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var (sucesso, mensagem, resultado) = await _authService.RefreshTokenAsync(command);
            if (!sucesso) return Unauthorized(new { mensagem });
            return Ok(resultado);
        }

        /// <summary>Solicita recuperação de senha por e-mail.</summary>
        [HttpPost("recuperar-senha")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaCommand command)
        {
            var (_, mensagem) = await _authService.RecuperarSenhaAsync(command);
            return Ok(new { mensagem });
        }

        /// <summary>Redefine a senha usando o token recebido por e-mail.</summary>
        [HttpPost("redefinir-senha")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaCommand command)
        {
            var (sucesso, mensagem) = await _authService.RedefinirSenhaAsync(command);
            if (!sucesso) return BadRequest(new { mensagem });
            return Ok(new { mensagem });
        }
    }
}
