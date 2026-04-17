using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Profiles.CreateProfile;
using MusicianFinder.Application.Features.Profiles.DeleteProfile;
using MusicianFinder.Application.Features.Profiles.GetMedia;
using MusicianFinder.Application.Features.Profiles.GetMyProfile;
using MusicianFinder.Application.Features.Profiles.GetNotificationSettings;
using MusicianFinder.Application.Features.Profiles.GetProfileById;
using MusicianFinder.Application.Features.Profiles.SearchProfiles;
using MusicianFinder.Application.Features.Profiles.UpdateAvatar;
using MusicianFinder.Application.Features.Profiles.UpdateProfile;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер профилей музыкантов.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ProfilesController"/>.
        /// </summary>
        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Поиск профилей.
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchProfilesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить профиль текущего пользователя.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _mediator.Send(new GetMyProfileQuery());
            return Ok(result);
        }

        /// <summary>
        /// Получить профиль по ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProfileByIdQuery { ProfileId = id });
            return Ok(result);
        }

        /// <summary>
        /// Создать профиль.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProfileCommand command)
        {
            var profileId = await _mediator.Send(command);
            return Ok(new { id = profileId });
        }

        /// <summary>
        /// Обновить профиль.
        /// </summary>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateProfileCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Удалить профиль.
        /// </summary>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            await _mediator.Send(new DeleteProfileCommand());
            return Ok(new { success = true });
        }

        /// <summary>
        /// Загрузить аватар.
        /// </summary>
        [HttpPost("avatar")]
        [Authorize]
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
        /// Получить медиа портфолио.
        /// </summary>
        [HttpGet("{id}/media")]
        public async Task<IActionResult> GetMedia(Guid id)
        {
            var result = await _mediator.Send(new GetMediaQuery { ProfileId = id });
            return Ok(result);
        }

        /// <summary>
        /// Получить настройки уведомлений.
        /// </summary>
        [HttpGet("notification-settings")]
        [Authorize]
        public async Task<IActionResult> GetNotificationSettings()
        {
            var result = await _mediator.Send(new GetNotificationSettingsQuery());
            return Ok(result);
        }
    }
}