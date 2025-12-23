using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.Services.Interfaces;
using backend.Models.Classes;
using backend.Services;
using backend.Models.Repositories.Interfaces;

namespace backend.Controllers;

/// <summary>
/// Контроллер загрузки файлов
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IProfileService _profileService;
    private readonly IAudioUploadService _audioService;
    private readonly IVideoUploadService _videoService;

    public UploadsController(IUserService userService, IProfileService profileService, IAudioUploadService audioUploadService, IVideoUploadService videoUploadService)
    {
        _userService = userService;
        _profileService = profileService;
        _audioService = audioUploadService;
        _videoService = videoUploadService;
    }

    /// <summary>
    /// Загрузить аватар
    /// </summary>
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
            return BadRequest("Файл не выбран");

        if (!avatar.ContentType.StartsWith("image/"))
            return BadRequest("Разрешены только изображения");

        if (avatar.Length > 2 * 1024 * 1024) // 2 МБ
            return BadRequest("Файл слишком большой");

        using var memoryStream = new MemoryStream();
        await avatar.CopyToAsync(memoryStream);
        var avatarBytes = memoryStream.ToArray();

        var userId = GetUserId();
        var success = await _userService.UpdateAvatarAsync(userId, avatarBytes);

        return success
            ? Ok(new { success = true })
            : BadRequest("Не удалось обновить аватар");
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }

    // В UploadsController.cs

    [HttpPost("portfolio/audio")]
    public async Task<IActionResult> UploadAudio(
         IFormFile audio,
         [FromForm] string title,
         [FromForm] string? description = null)
    {
        try
        {
            var userId = GetUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return BadRequest("Профиль не найден");
            var profileId = Guid.Parse(profile.RootElement.GetProperty("id").GetString()!);

            var result = await _audioService.UploadAudioAsync(profileId, audio, title, description);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Загрузить видео в портфолио
    /// </summary>
    [HttpPost("portfolio/video")]
    public async Task<IActionResult> UploadVideo(
        IFormFile video,
        [FromForm] string title,
        [FromForm] string? description = null)
    {
        try
        {
            var userId = GetUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return BadRequest("Профиль не найден");
            var profileId = Guid.Parse(profile.RootElement.GetProperty("id").GetString()!);

            var result = await _videoService.UploadVideoAsync(profileId, video, title, description);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}