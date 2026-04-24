using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Auth;
using MusicianFinder.Application.DTOs.Auth;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер аутентификации.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AuthController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Запрос кода подтверждения на email.
        /// </summary>
        /// <param name="command">Команда с email.</param>
        /// <returns>Статус 200 OK.</returns>
        [HttpPost("request-code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestCode([FromBody] RequestCodeCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Вход/регистрация по коду подтверждения.
        /// </summary>
        /// <param name="command">Команда с email и кодом.</param>
        /// <returns>Данные аутентификации.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}