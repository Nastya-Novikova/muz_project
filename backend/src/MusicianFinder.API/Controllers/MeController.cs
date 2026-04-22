using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Application.Commands.Notifications;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Commands.Users;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Queries.Events;
using MusicianFinder.Application.Queries.Notifications;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.Application.Queries.Suggestions;
using MusicianFinder.Application.Queries.Users;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для операций, связанных с текущим аутентифицированным пользователем.
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

        /// <summary>
        /// Получить профиль текущего пользователя.
        /// </summary>
        /// <returns>Профиль музыканта.</returns>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _mediator.Send(new GetMyProfileQuery());
            return Ok(result);
        }

        /// <summary>
        /// Обновить профиль текущего пользователя.
        /// </summary>
        /// <param name="command">Данные для обновления.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Мягкое удаление профиля текущего пользователя.
        /// </summary>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMyProfile()
        {
            await _mediator.Send(new DeleteProfileCommand());
            return NoContent();
        }

        /// <summary>
        /// Загрузить или обновить аватар профиля.
        /// </summary>
        /// <param name="avatar">Файл изображения.</param>
        /// <returns>URL загруженного аватара.</returns>
        [HttpPost("avatar")]
        [ProducesResponseType(typeof(AvatarUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            var command = new UpdateAvatarCommand
            {
                FileStream = avatar.OpenReadStream(),
                FileName = avatar.FileName,
                ContentType = avatar.ContentType
            };
            var url = await _mediator.Send(command);
            return Ok(new { avatarUrl = url });
        }

        /// <summary>
        /// Получить список избранных профилей текущего пользователя.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с избранными профилями.</returns>
        [HttpGet("favorites")]
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
        [HttpPost("favorites/{profileId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddFavorite(Guid profileId)
        {
            await _mediator.Send(new AddFavoriteCommand { ProfileId = profileId });
            return NoContent();
        }

        /// <summary>
        /// Удалить профиль из избранного.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("favorites/{profileId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite(Guid profileId)
        {
            await _mediator.Send(new RemoveFavoriteCommand { ProfileId = profileId });
            return NoContent();
        }

        /// <summary>
        /// Получить мероприятия, созданные текущим пользователем.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с мероприятиями.</returns>
        [HttpGet("events/created")]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCreatedEvents([FromQuery] GetMyCreatedEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить мероприятия, на которые записан текущий пользователь.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с мероприятиями.</returns>
        [HttpGet("events/registered")]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyRegisteredEvents([FromQuery] GetMyRegisteredEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить входящие предложения о сотрудничестве.
        /// </summary>
        /// <param name="query">Параметры пагинации и сортировки.</param>
        /// <returns>Страница с предложениями.</returns>
        [HttpGet("suggestions/received")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReceivedSuggestions([FromQuery] GetReceivedSuggestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить исходящие предложения о сотрудничестве.
        /// </summary>
        /// <param name="query">Параметры пагинации и сортировки.</param>
        /// <returns>Страница с предложениями.</returns>
        [HttpGet("suggestions/sent")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentSuggestions([FromQuery] GetSentSuggestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список уведомлений текущего пользователя.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с уведомлениями.</returns>
        [HttpGet("notifications")]
        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Отметить одно уведомление как прочитанное.
        /// </summary>
        /// <param name="id">Идентификатор уведомления.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPatch("notifications/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkNotificationAsRead(Guid id)
        {
            await _mediator.Send(new MarkNotificationAsReadCommand { NotificationId = id });
            return NoContent();
        }

        /// <summary>
        /// Отметить все уведомления как прочитанные.
        /// </summary>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPatch("notifications/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            await _mediator.Send(new MarkAllNotificationsAsReadCommand());
            return NoContent();
        }

        /// <summary>
        /// Получить количество непрочитанных уведомлений.
        /// </summary>
        /// <returns>Количество непрочитанных уведомлений.</returns>
        [HttpGet("notifications/unread-count")]
        [ProducesResponseType(typeof(UnreadCountResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _mediator.Send(new GetUnreadCountQuery());
            return Ok(new { unreadCount = count });
        }

        /// <summary>
        /// Загрузить медиафайл в портфолио текущего пользователя.
        /// </summary>
        /// <param name="file">Файл.</param>
        /// <param name="title">Название.</param>
        /// <param name="type">Тип медиа.</param>
        /// <param name="description">Описание (опционально).</param>
        /// <returns>Идентификатор созданного медиа.</returns>
        [HttpPost("profile/media")]
        [ProducesResponseType(typeof(MediaUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMedia(
            IFormFile file,
            [FromForm] string title,
            [FromForm] MediaType type,
            [FromForm] string? description = null)
        {
            var command = new UploadMediaCommand
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Title = title,
                Description = description,
                Type = type
            };
            var mediaId = await _mediator.Send(command);
            return Ok(new { id = mediaId });
        }

        /// <summary>
        /// Удалить медиафайл из портфолио текущего пользователя.
        /// </summary>
        /// <param name="mediaId">Идентификатор медиа.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("profile/media/{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMedia(Guid mediaId)
        {
            await _mediator.Send(new DeleteMediaCommand { MediaId = mediaId });
            return NoContent();
        }

        private class AvatarUploadResponse
        {
            public string AvatarUrl { get; set; } = string.Empty;
        }

        private class UnreadCountResponse
        {
            public int UnreadCount { get; set; }
        }

        private class MediaUploadResponse
        {
            public Guid Id { get; set; }
        }
    }
}