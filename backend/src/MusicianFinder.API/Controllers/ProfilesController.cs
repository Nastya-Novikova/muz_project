using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Queries.Profiles;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с профилями музыкантов.
    /// </summary>
    [ApiController]
    [Route("api/profiles")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ProfilesController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Поиск и фильтрация профилей.
        /// </summary>
        /// <param name="query">Параметры поиска, фильтрации и пагинации.</param>
        /// <returns>Страница с профилями.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] SearchProfilesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Создать профиль музыканта (для нового пользователя).
        /// </summary>
        /// <param name="command">Данные для создания профиля.</param>
        /// <returns>Идентификатор созданного профиля.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProfileCommand command)
        {
            SetIdempotencyKey(command);
            var profileId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = profileId }, new { id = profileId });
        }

        /// <summary>
        /// Получить профиль по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор профиля.</param>
        /// <returns>Профиль музыканта.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProfileByIdQuery { ProfileId = id });
            return Ok(result);
        }

        /// <summary>
        /// Получить медиа-файлы портфолио указанного профиля.
        /// </summary>
        /// <param name="id">Идентификатор профиля.</param>
        /// <returns>Структура с аудио, видео и фото.</returns>
        [HttpGet("{id:guid}/media")]
        [ProducesResponseType(typeof(MediaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMedia(Guid id)
        {
            var result = await _mediator.Send(new GetMediaQuery { ProfileId = id });
            return Ok(result);
        }

        private void SetIdempotencyKey(IBaseCommand command)
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                command.IdempotencyKey = key;
        }
    }
}