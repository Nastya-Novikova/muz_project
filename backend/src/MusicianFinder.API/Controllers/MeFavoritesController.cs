using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Users;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Queries.Users;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для управления избранными профилями текущего пользователя.
    /// </summary>
    [ApiController]
    [Route("api/me/favorites")]
    [Authorize]
    public class MeFavoritesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MeFavoritesController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public MeFavoritesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить список избранных профилей текущего пользователя.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с избранными профилями.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavorites([FromQuery] GetFavoritesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Добавить профиль в избранное.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPost("{profileId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddFavorite(Guid profileId)
        {
            var command = new AddFavoriteCommand { ProfileId = profileId };
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Удалить профиль из избранного.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("{profileId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite(Guid profileId)
        {
            var command = new RemoveFavoriteCommand { ProfileId = profileId };
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        private void SetIdempotencyKey(IBaseCommand command)
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                command.IdempotencyKey = key;
        }
    }
}