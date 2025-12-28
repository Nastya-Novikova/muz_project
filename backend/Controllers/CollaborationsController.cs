using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;

namespace backend.Controllers;

/// <summary>
/// Контроллер предложений о сотрудничестве
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollaborationsController : ControllerBase
{
    private readonly ICollaborationService _service;

    public CollaborationsController(ICollaborationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Отправить предложение о сотрудничестве
    /// </summary>
    [HttpPost("{userId}")]
    public async Task<IActionResult> SendSuggestion(Guid userId, [FromBody] JsonDocument objJson)
    {
        var fromUserId = GetUserId();
        var message = objJson.RootElement.TryGetProperty("message", out var m) ? m.GetString() : null;
        var result = await _service.SendSuggestionAsync(fromUserId, userId, message);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить предложения мне
    /// </summary>
    [HttpGet("received")]
    public async Task<IActionResult> GetReceived([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? sortBy = "createdAt", [FromQuery] bool sortDesc = true)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { page, limit, sortBy, sortDesc }));
        var userId = GetUserId();
        var result = await _service.GetReceivedAsync(userId, queryParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить мои предложения
    /// </summary>
    [HttpGet("sent")]
    public async Task<IActionResult> GetSent([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? sortBy = "createdAt", [FromQuery] bool sortDesc = true)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { page, limit, sortBy, sortDesc }));
        var userId = GetUserId();
        var result = await _service.GetSentAsync(userId, queryParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Проверить, отправлено ли пользователю предложение
    /// </summary>
    [HttpGet("{collaboratedProfileId}")]
    public async Task<IActionResult> IsCollaborated(Guid collaboratedProfileId)
    {
        var userId = GetUserId();
        var isCollaborated = await _service.IsCollaboratedAsync(userId, collaboratedProfileId);
        return Ok(new { isCollaborated });
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }
}