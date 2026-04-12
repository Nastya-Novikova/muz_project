using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Services;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Favorites;
using AutoMapper;

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
    public async Task<ActionResult<PagedResult<FavoriteProfileDto>>> GetFavorites(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20)
    {
        var userId = GetUserId();
        var result = await _service.GetFavoritesAsync(userId, page, limit);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Добавить в избранное
    /// </summary>
    [HttpPost("{profileId}")]
    public async Task<IActionResult> Add(Guid profileId)
    {
        var userId = GetUserId();
        var result = await _service.AddFavoriteAsync(userId, profileId);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { success = true });
    }

    /// <summary>
    /// Удалить из избранного
    /// </summary>
    [HttpDelete("{profileId}")]
    public async Task<IActionResult> Remove(Guid profileId)
    {
        var userId = GetUserId();
        var result = await _service.RemoveFavoriteAsync(userId, profileId);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { success = true });
    }

    /// <summary>
    /// Проверить, добавлен ли пользователь в избранное
    /// </summary>
    [HttpGet("{profileId}/is-favorite")]
    public async Task<ActionResult<bool>> IsFavorite(Guid profileId)
    {
        var userId = GetUserId();
        var result = await _service.IsFavoriteAsync(userId, profileId);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { isFavorite = result.Value });
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }
}