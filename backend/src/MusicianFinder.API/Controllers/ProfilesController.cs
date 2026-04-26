using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.API.Contracts.Responses;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Управление профилями музыкантов.
    /// </summary>
    [ApiController]
    [Route("api/profiles")]
    public class MusicianProfilesController : BaseApiController
    {
        /// <summary>
        /// Поиск и фильтрация профилей.
        /// </summary>
        /// <param name="query">Параметры поиска.</param>
        /// <returns>Страница с профилями.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] SearchProfilesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить профиль по идентификатору.
        /// В ответе присутствуют секции audio, video, photos с портфолио.
        /// </summary>
        /// <param name="id">Идентификатор профиля.</param>
        /// <returns>Профиль музыканта с медиа-вложениями.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetProfileByIdQuery { ProfileId = id });
            return Ok(result);
        }

        /// <summary>
        /// Получить профиль текущего пользователя.
        /// В ответе присутствуют секции audio, video, photos с портфолио.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await Mediator.Send(new GetMyProfileQuery());
            return Ok(result);
        }

        /// <summary>
        /// Создать профиль музыканта.
        /// </summary>
        /// <param name="command">Данные для создания профиля.</param>
        /// <returns>Идентификатор созданного профиля.</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProfileCommand command)
        {
            SetIdempotencyKey(command);
            var profileId = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = profileId }, new { id = profileId });
        }

        /// <summary>
        /// Частичное обновление своего профиля.
        /// Передаются только те поля, которые необходимо изменить.
        /// </summary>
        /// <param name="command">Данные для обновления.</param>
        [Authorize]
        [HttpPatch("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileCommand command)
        {
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Мягкое удаление своего профиля.
        /// </summary>
        [Authorize]
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var command = new DeleteProfileCommand();
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Загрузить или обновить аватар своего профиля.
        /// </summary>
        /// <param name="avatar">Файл изображения (JPEG, PNG, GIF).</param>
        /// <returns>URL загруженного аватара.</returns>
        [Authorize]
        [HttpPut("me/avatar")]
        [ProducesResponseType(typeof(FileUploadResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            using var ms = new MemoryStream();
            await avatar.CopyToAsync(ms);
            var command = new UpdateAvatarCommand
            {
                Content = ms.ToArray(),
                FileName = avatar.FileName,
                ContentType = avatar.ContentType
            };
            SetIdempotencyKey(command);
            var url = await Mediator.Send(command);
            return Ok(new FileUploadResultDto { Url = url });
        }

        /// <summary>
        /// Загрузить медиафайл (аудио, видео, фото) в своё портфолио.
        /// </summary>
        /// <param name="file">Файл.</param>
        /// <param name="title">Название.</param>
        /// <param name="type">Тип медиа (Audio, Video, Photo).</param>
        /// <param name="description">Описание (опционально).</param>
        /// <returns>Идентификатор созданного медиа.</returns>
        [Authorize]
        [HttpPost("me/media")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadMedia(
            IFormFile file,
            [FromForm] string title,
            [FromForm] MediaType type,
            [FromForm] string? description = null)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var command = new UploadMediaCommand
            {
                Content = ms.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Title = title,
                Description = description,
                Type = type
            };
            SetIdempotencyKey(command);
            var mediaId = await Mediator.Send(command);
            return Ok(new { id = mediaId });
        }

        /// <summary>
        /// Удалить медиафайл из своего портфолио.
        /// </summary>
        /// <param name="mediaId">Идентификатор медиа.</param>
        [Authorize]
        [HttpDelete("me/media/{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMedia(Guid mediaId)
        {
            var command = new DeleteMediaCommand { MediaId = mediaId };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }
    }
}