using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<JsonDocument> Search([FromQuery] string? city, [FromQuery] string? genre)
        {
            var json = await _profileService.SearchAsync(city, genre);
            return JsonDocument.Parse(json);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var json = await _profileService.GetByIdAsync(id);
            return json == null ? NotFound() : Ok(JsonDocument.Parse(json));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonDocument jsonDocument)
        {
            var resultJson = await _profileService.CreateAsync(jsonDocument);
            var doc = JsonDocument.Parse(resultJson);
            var id = Guid.Parse(doc.RootElement.GetProperty("id").GetString()!);
            return CreatedAtAction(nameof(Get), new { id }, doc.RootElement);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _profileService.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }
    }
}
