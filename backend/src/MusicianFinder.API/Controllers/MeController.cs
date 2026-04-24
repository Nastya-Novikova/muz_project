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
    [Authorize]
    public class MeController : BaseApiController
    {
        /// <summary>
        /// Получить базовую информацию о текущем пользователе.
        /// </summary>
        /// <returns>Email и роль пользователя.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await Mediator.Send(new GetCurrentUserQuery());
            return Ok(result);
        }
    }
}