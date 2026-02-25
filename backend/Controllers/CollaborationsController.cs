using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Common;

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
    [HttpPost("{profileId}")]
    public async Task<IActionResult> SendSuggestion(Guid profileId, [FromBody] SendSuggestionRequest request)
    {
        if (profileId != request.ToProfileId)
            return BadRequest(new { error = "Profile ID mismatch" });

        var userId = GetUserId();
        var result = await _service.SendSuggestionAsync(userId, request.ToProfileId, request.Message);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { success = true });
    }

    /// <summary>
    /// Получить предложения мне
    /// </summary>
    [HttpGet("received")]
    public async Task<ActionResult<PagedResult<SuggestionDto>>> GetReceived(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] bool sortDesc = true)
    {
        var userId = GetUserId();
        var result = await _service.GetReceivedAsync(userId, page, limit, sortBy, sortDesc);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Получить мои предложения
    /// </summary>
    [HttpGet("sent")]
    public async Task<ActionResult<PagedResult<SuggestionDto>>> GetSent(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] bool sortDesc = true)
    {
        var userId = GetUserId();
        var result = await _service.GetSentAsync(userId, page, limit, sortBy, sortDesc);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Проверить, отправлено ли пользователю предложение
    /// </summary>
    [HttpGet("{collaboratedProfileId}")]
    public async Task<ActionResult<bool>> IsCollaborated(Guid collaboratedProfileId)
    {
        var userId = GetUserId();
        var result = await _service.IsCollaboratedAsync(userId, collaboratedProfileId);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { isCollaborated = result.Value });
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }
}