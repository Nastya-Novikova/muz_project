using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.Services.Interfaces;
using backend.Models.Classes;
using backend.Services;
using backend.Models.Repositories.Interfaces;
using backend.Models.DTOs.Uploads;

namespace backend.Controllers;

/// <summary>
/// Контроллер загрузки файлов
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IAudioUploadService _audioService;
    private readonly IVideoUploadService _videoService;
    private readonly IPhotoUploadService _photoService;

    public UploadsController(IProfileService profileService, IAudioUploadService audioUploadService, IVideoUploadService videoUploadService, IPhotoUploadService photoUploadService)
    {
        _profileService = profileService;
        _audioService = audioUploadService;
        _videoService = videoUploadService;
        _photoService = photoUploadService;
    }

    /// <summary>
    /// Загрузить аватар
    /// </summary>
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        try
        {
            var userId = GetUserId();
            using var stream = avatar.OpenReadStream();
            var result = await _profileService.UpdateAvatarAsync(userId, stream, avatar.FileName, avatar.ContentType);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { success = true, avatarUrl = result.Value });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Загрузить аудио в портфолио
    /// </summary>
    [HttpPost("portfolio/audio")]
    public async Task<ActionResult<UploadResultDto>> UploadAudio(
         IFormFile audio,
         [FromForm] string title,
         [FromForm] string? description = null)
    {
        try
        {
            var userId = GetUserId();
            using var stream = audio.OpenReadStream();
            var result = await _audioService.UploadAudioAsync(userId, stream, audio.FileName, audio.ContentType, title, description);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Загрузить видео в портфолио
    /// </summary>
    [HttpPost("portfolio/video")]
    public async Task<ActionResult<UploadResultDto>> UploadVideo(
        IFormFile video,
        [FromForm] string title,
        [FromForm] string? description = null)
    {
        try
        {
            var userId = GetUserId();
            using var stream = video.OpenReadStream();
            var result = await _videoService.UploadVideoAsync(userId, stream, video.FileName, video.ContentType, title, description);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Загрузить фото в портфолио
    /// </summary>
    [HttpPost("portfolio/photo")]
    public async Task<ActionResult<UploadResultDto>> UploadPhoto(
    IFormFile photo,
    [FromForm] string title,
    [FromForm] string? description = null)
    {
        try
        {
            var userId = GetUserId();
            using var stream = photo.OpenReadStream();
            var result = await _photoService.UploadPhotoAsync(userId, stream, photo.FileName, photo.ContentType, title, description);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }
}