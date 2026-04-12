using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Services;
using backend.Models.Classes;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Vk;
using backend.Models.DTOs.Profiles;

namespace backend.Controllers;

/// <summary>
/// Контроллер профилей
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _service;
    private readonly IVkAuthService _vkAuthService;

    public ProfilesController(IProfileService service, IVkAuthService vkAuthService)
    {
        _service = service;
        _vkAuthService = vkAuthService;
    }

    /// <summary>
    /// Поиск музыкантов
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<PagedResult<ProfileDto>>> Search([FromBody] SearchRequest request)
    {
        var result = await _service.SearchAsync(request);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Получить свой профиль
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ProfileDto>> GetMyProfile()
    {
        var userId = GetUserId();
        var result = await _service.GetByUserIdAsync(userId);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Получить профиль по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileDto>> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Создать профиль
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProfileDto>> Create([FromBody] CreateProfileRequest request)
    {
        var userId = GetUserId();
        var result = await _service.CreateAsync(userId, request);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Обновить профиль
    /// </summary>
    [HttpPut]
    [Authorize]
    public async Task<ActionResult<ProfileDto>> Update([FromBody] UpdateProfileRequest request)
    {
        var userId = GetUserId();
        var result = await _service.UpdateAsync(userId, request);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Получить медиа контент портфолио пользователя
    /// </summary>
    [HttpGet("{id}/media")]
    [Authorize]
    public async Task<ActionResult<object>> GetMedia(Guid id)
    {
        var result = await _service.GetMediaAsync(id);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Удалить профиль (soft-delete)
    /// </summary>
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var userId = GetUserId();
        var result = await _service.DeleteAsync(userId);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(new { success = true });
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }

    /// <summary>
    /// Привязать аккаунт ВКонтакте
    /// </summary>
    [HttpPost("connect-vk")]
    [Authorize]
    public async Task<IActionResult> ConnectVk([FromBody] ConnectVkRequest request)
    {
        var userId = GetUserId();
        
        var result = await _vkAuthService.ConnectVkAsync(userId, request.Code, request.CodeVerifier, request.DeviceId);
        
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        
        return Ok(new { success = true });
    }

    /// <summary>
    /// Тестовый эндпоинт для отправки сообщения (только для отладки)
    /// </summary>
    [HttpPost("test-notification")]
    [Authorize]
    public async Task<IActionResult> TestNotification([FromBody] string message)
    {
        var userId = GetUserId();

        var result = await _vkAuthService.SendNotificationAsync(userId, message ?? "Тестовое уведомление от MusicianFinder!");
        
        if (!result)
            return BadRequest(new { error = "Failed to send message. Check logs for details." });
        
        return Ok(new { success = true, message = "Notification sent successfully" });
    }
}