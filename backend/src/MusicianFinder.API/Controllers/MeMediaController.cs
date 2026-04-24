using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для управления медиа-файлами портфолио текущего пользователя.
    /// </summary>
    [ApiController]
    [Route("api/me/profile/media")]
    [Authorize]
    public class MeMediaController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MeMediaController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public MeMediaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Загрузить медиафайл в портфолио текущего пользователя.
        /// </summary>
        /// <param name="file">Файл.</param>
        /// <param name="title">Название.</param>
        /// <param name="type">Тип медиа.</param>
        /// <param name="description">Описание (опционально).</param>
        /// <returns>Идентификатор созданного медиа.</returns>
        [HttpPost]
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
            var mediaId = await _mediator.Send(command);
            return Ok(new { id = mediaId });
        }

        /// <summary>
        /// Удалить медиафайл из портфолио текущего пользователя.
        /// </summary>
        /// <param name="mediaId">Идентификатор медиа.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("{mediaId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMedia(Guid mediaId)
        {
            var command = new DeleteMediaCommand { MediaId = mediaId };
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