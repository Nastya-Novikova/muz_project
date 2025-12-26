using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Services;
using backend.Models.Classes;

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
    public async Task<IActionResult> Search([FromBody] JsonDocument? searchParams)
    {
        var result = await _service.SearchAsync(searchParams ?? JsonDocument.Parse("{}"));
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить свой профиль
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetUserId();
        var obj = await _service.GetByUserIdAsync(userId);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Получить профиль по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var obj = await _service.GetByIdAsync(id);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Создать профиль
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] JsonDocument objJson)
    {
        var userId = GetUserId();
        var obj = await _service.CreateAsync(objJson, userId);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Обновить профиль
    /// </summary>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] JsonDocument objJson)
    {
        var userId = GetUserId();
        var obj = await _service.UpdateAsync(objJson, userId);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Удалить профиль (soft-delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var obj = await _service.DeleteAsync(id);
        return obj != null ? Ok(obj) : BadRequest();
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }
}