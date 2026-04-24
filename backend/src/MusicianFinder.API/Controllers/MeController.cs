using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Queries.Users;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для операций над текущим пользователем.
    /// </summary>
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MeController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public MeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить базовую информацию о текущем пользователе.
        /// </summary>
        /// <returns>Email и роль пользователя.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _mediator.Send(new GetCurrentUserQuery());
            return Ok(result);
        }
    }
}