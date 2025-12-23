using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;

namespace backend.Controllers;

/// <summary>
/// Контроллер профилей
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    public async Task<IActionResult> Search([FromBody] JsonDocument searchParams)
    {
        var result = await _service.SearchAsync(searchParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить свой профиль
    /// </summary>
    [HttpGet]
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
    public async Task<IActionResult> Update([FromBody] JsonDocument objJson)
    {
        var userId = GetUserId();
        var obj = await _service.UpdateAsync(Guid.Empty, objJson, userId); // ID из JWT
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Удалить профиль (soft-delete)
    /// </summary>
    [HttpDelete("{id}")]
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