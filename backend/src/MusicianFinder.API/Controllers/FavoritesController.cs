using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Favorites.AddFavorite;
using MusicianFinder.Application.Features.Favorites.CheckIsFavorite;
using MusicianFinder.Application.Features.Favorites.GetFavorites;
using MusicianFinder.Application.Features.Favorites.RemoveFavorite;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер избранного.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FavoritesController"/>.
        /// </summary>
        public FavoritesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить избранные профили.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFavorites([FromQuery] GetFavoritesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Добавить в избранное.
        /// </summary>
        [HttpPost("{profileId}")]
        public async Task<IActionResult> Add(Guid profileId)
        {
            await _mediator.Send(new AddFavoriteCommand { ProfileId = profileId });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Удалить из избранного.
        /// </summary>
        [HttpDelete("{profileId}")]
        public async Task<IActionResult> Remove(Guid profileId)
        {
            await _mediator.Send(new RemoveFavoriteCommand { ProfileId = profileId });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Проверить, в избранном ли профиль.
        /// </summary>
        [HttpGet("{profileId}/is-favorite")]
        public async Task<IActionResult> CheckIsFavorite(Guid profileId)
        {
            var result = await _mediator.Send(new CheckIsFavoriteQuery { ProfileId = profileId });
            return Ok(new { isFavorite = result });
        }
    }
}