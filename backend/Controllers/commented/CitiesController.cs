/*using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly ICityService _service;

    public CitiesController(ICityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<JsonDocument> Get([FromQuery] string? query = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDesc = false)
    {
        var queryParams = JsonDocument.Parse(JsonSerializer.Serialize(new { query, sortBy, sortDesc }));
        return (await _service.GetAllAsync(queryParams)) ?? JsonDocument.Parse("[]");
    }
}*/