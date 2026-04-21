using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Auth.Login;
using MusicianFinder.Application.Features.Auth.RequestCode;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер аутентификации.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AuthController"/>.
        /// </summary>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Запрос кода подтверждения на email.
        /// </summary>
        [HttpPost("request-code")]
        public async Task<IActionResult> RequestCode([FromBody] RequestCodeCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Вход/регистрация по коду.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}