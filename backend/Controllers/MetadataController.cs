using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Models.DTOs;

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
    public async Task<ActionResult<List<LookupItemDto>>> GetCities([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var result = await _cityService.GetAllAsync(query, sortBy, sortDesc);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Получить виды деятельности (музыкальные специальности)
    /// </summary>
    [HttpGet("activities")]
    public async Task<ActionResult<List<LookupItemDto>>> GetActivities([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var result = await _specialtyService.GetAllAsync(query, sortBy, sortDesc);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Получить музыкальные жанры
    /// </summary>
    [HttpGet("genres")]
    public async Task<ActionResult<List<LookupItemDto>>> GetGenres([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var result = await _genreService.GetAllAsync(query, sortBy, sortDesc);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Получить статусы поиска (цели сотрудничества)
    /// </summary>
    [HttpGet("statuses")]
    public async Task<ActionResult<List<LookupItemDto>>> GetStatuses([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var result = await _goalService.GetAllAsync(query, sortBy, sortDesc);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}