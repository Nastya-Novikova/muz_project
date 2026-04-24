using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Profiles;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.API.Contracts.Responses;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для управления профилем текущего пользователя.
    /// </summary>
    [ApiController]
    [Route("api/me/profile")]
    [Authorize]
    public class MeProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MeProfileController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public MeProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить профиль текущего пользователя.
        /// </summary>
        /// <returns>Профиль музыканта.</returns>
        [HttpGet]
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
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileCommand command)
        {
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Мягкое удаление профиля текущего пользователя.
        /// </summary>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var command = new DeleteProfileCommand();
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Загрузить или обновить аватар профиля.
        /// </summary>
        /// <param name="avatar">Файл изображения.</param>
        /// <returns>URL загруженного аватара.</returns>
        [HttpPost("avatar")]
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
            var url = await _mediator.Send(command);
            return Ok(new FileUploadResultDto { Url = url });
        }

        private void SetIdempotencyKey(IBaseCommand command)
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                command.IdempotencyKey = key;
        }
    }
}