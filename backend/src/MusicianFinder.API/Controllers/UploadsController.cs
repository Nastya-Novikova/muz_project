using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Uploads.UploadAudio;
using MusicianFinder.Application.Features.Uploads.UploadPhoto;
using MusicianFinder.Application.Features.Uploads.UploadVideo;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер загрузки файлов.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadsController"/>.
        /// </summary>
        public UploadsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Загрузить аудио.
        /// </summary>
        [HttpPost("audio")]
        public async Task<IActionResult> UploadAudio(
            IFormFile audio,
            [FromForm] string title,
            [FromForm] string? description = null)
        {
            var command = new UploadAudioCommand
            {
                FileStream = audio.OpenReadStream(),
                FileName = audio.FileName,
                ContentType = audio.ContentType,
                Title = title,
                Description = description
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Загрузить видео.
        /// </summary>
        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(
            IFormFile video,
            [FromForm] string title,
            [FromForm] string? description = null)
        {
            var command = new UploadVideoCommand
            {
                FileStream = video.OpenReadStream(),
                FileName = video.FileName,
                ContentType = video.ContentType,
                Title = title,
                Description = description
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Загрузить фото.
        /// </summary>
        [HttpPost("photo")]
        public async Task<IActionResult> UploadPhoto(
            IFormFile photo,
            [FromForm] string title,
            [FromForm] string? description = null)
        {
            var command = new UploadPhotoCommand
            {
                FileStream = photo.OpenReadStream(),
                FileName = photo.FileName,
                ContentType = photo.ContentType,
                Title = title,
                Description = description
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}