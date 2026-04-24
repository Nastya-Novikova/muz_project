using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Auth;
using MusicianFinder.Application.DTOs.Auth;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер аутентификации.
    /// </summary>
    public class AuthController : BaseApiController
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AuthController"/>.
        /// </summary>
        public AuthController() { }

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
            await Mediator.Send(command);
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
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}