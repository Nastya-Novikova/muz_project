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
//[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ICollaborationService _collaborationService;
    private readonly IFavoriteService _favoriteService;

    public ProfilesController(IProfileService profileService, ICollaborationService collaborationService, IFavoriteService favoriteService)
    {
        _profileService = profileService;
        _collaborationService = collaborationService;
        _favoriteService = favoriteService;
    }

    /// <summary>
    /// Поиск музыкантов
    /// </summary>
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] JsonDocument? searchParams)
    {
        var result = await _profileService.SearchAsync(searchParams ?? JsonDocument.Parse("{}"));
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить свой профиль
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetUserId();
        var obj = await _profileService.GetByUserIdAsync(userId);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Получить профиль по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var obj = await _profileService.GetByIdAsync(id);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Создать профиль
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JsonDocument objJson)
    {
        var userId = GetUserId();
        var obj = await _profileService.CreateAsync(objJson, userId);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Обновить профиль
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] JsonDocument objJson)
    {
        var userId = GetUserId();
        var obj = await _profileService.UpdateAsync(objJson, userId);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /// <summary>
    /// Удалить профиль (soft-delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var obj = await _profileService.DeleteAsync(id);
        return obj != null ? Ok(obj) : BadRequest();
    }

    /*[HttpGet("full")]
    public async Task<IActionResult> GetFullProfile()
    {
        var userId = GetUserId();
        var result = await _profileService.GetFullProfileAsync(userId);
        if (result == null) return NotFound("Profile not found");
        return Ok(result);
    }*/

    /*[HttpGet("full")]
    [Authorize]
    public async Task<IActionResult> GetFullProfile()
    {
        var userId = GetUserId();

        // Получаем свой профиль
        var profile = await _profileService.GetFullProfileAsync(userId);
        if (profile == null) return BadRequest();

        // Получаем предложения
        var received = await _collaborationService.GetReceivedAsync(userId, JsonDocument.Parse("{}"));
        var sent = await _collaborationService.GetSentAsync(userId, JsonDocument.Parse("{}"));

        // Получаем избранные ID
        var favorites = await _favoriteService.GetFavoritesAsync(userId, JsonDocument.Parse("{}"));

        var fullProfile = new
        {
            profile = profile.RootElement,
            collaborations = new
            {
                received = received?.RootElement.GetProperty("suggestions") ?? JsonDocument.Parse("[]").RootElement,
                sent = sent?.RootElement.GetProperty("suggestions") ?? JsonDocument.Parse("[]").RootElement
            },
            favorites = favorites?.RootElement.GetProperty("favorites") ?? JsonDocument.Parse("[]").RootElement
        };

        return Ok(JsonDocument.Parse(JsonSerializer.Serialize(fullProfile)));
    }*/

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet("test-seed-data")]
    public async Task<IActionResult> TestSeedData()
    {
        try
        {
            // Тестовое письмо на реальный email
            var result = await _profileService.TestSeedData();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed: {ex.Message}");
        }
    }
}