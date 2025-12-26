using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Services;

namespace backend.Controllers;

/// <summary>
/// Контроллер избранных профилей
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _service;

    public FavoritesController(IFavoriteService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить избранные профили
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetFavorites([FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { page, limit }));
        var userId = GetUserId();
        var result = await _service.GetFavoritesAsync(userId, queryParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Добавить в избранное
    /// </summary>
    [HttpPost("{favoriteUserId}")]
    public async Task<IActionResult> Add(Guid favoriteUserId)
    {
        var userId = GetUserId();
        var result = await _service.AddAsync(userId, favoriteUserId);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Удалить из избранного
    /// </summary>
    [HttpDelete("{favoriteUserId}")]
    public async Task<IActionResult> Remove(Guid favoriteUserId)
    {
        var userId = GetUserId();
        var result = await _service.RemoveAsync(userId, favoriteUserId);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Проверить, добавлен ли пользователь в избранное
    /// </summary>
    [HttpGet("{favoriteUserId}")]
    public async Task<IActionResult> IsFavorite(Guid favoriteUserId)
    {
        var userId = GetUserId();
        var isFavorite = await _service.IsFavoriteAsync(userId, favoriteUserId);
        return Ok(new { isFavorite });
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }
}