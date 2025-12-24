using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend.Services.Interfaces;

namespace backend.Controllers;

/// <summary>
/// Контроллер метаданных (справочников)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    private readonly ICityService _cityService;
    private readonly IGenreService _genreService;
    private readonly IMusicalSpecialtyService _specialtyService;
    private readonly ICollaborationGoalService _goalService;

    public MetadataController(
        ICityService cityService,
        IGenreService genreService,
        IMusicalSpecialtyService specialtyService,
        ICollaborationGoalService goalService)
    {
        _cityService = cityService;
        _genreService = genreService;
        _specialtyService = specialtyService;
        _goalService = goalService;
    }

    /// <summary>
    /// Получить список городов
    /// </summary>
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { query, sortBy, sortDesc }));
        var result = await _cityService.GetAllAsync(queryParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить виды деятельности (музыкальные специальности)
    /// </summary>
    [HttpGet("activities")]
    public async Task<IActionResult> GetActivities([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { query, sortBy, sortDesc }));
        var result = await _specialtyService.GetAllAsync(queryParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить музыкальные жанры
    /// </summary>
    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { query, sortBy, sortDesc }));
        var result = await _genreService.GetAllAsync(queryParams);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Получить статусы поиска (цели сотрудничества)
    /// </summary>
    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { query, sortBy, sortDesc }));
        var result = await _goalService.GetAllAsync(queryParams);
        return result != null ? Ok(result) : BadRequest();
    }
}