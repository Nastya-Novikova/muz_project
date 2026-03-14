using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Services;
using backend.Models.Classes;
using backend.Models.DTOs.Common;
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

    public ProfilesController(IProfileService service)
    {
        _service = service;
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
}