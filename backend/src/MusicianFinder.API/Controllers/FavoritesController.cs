using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Favorites;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Queries.Favorites;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для управления избранными профилями текущего пользователя.
    /// </summary>
    [Authorize]
    [ApiController]
    public class FavoritesController : BaseApiController
    {
        /// <summary>
        /// Получить список избранных профилей.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с избранными профилями.</returns>
        [HttpGet("me/favorites")]
        [ProducesResponseType(typeof(PagedResult<ProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavorites([FromQuery] GetFavoritesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Добавить профиль в избранное.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPut("{profileId:guid}/favorite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddFavorite(Guid profileId)
        {
            var command = new AddFavoriteCommand { TargetProfileId = profileId };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Удалить профиль из избранного.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("{profileId:guid}/favorite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite(Guid profileId)
        {
            var command = new RemoveFavoriteCommand { TargetProfileId = profileId };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }
    }
}