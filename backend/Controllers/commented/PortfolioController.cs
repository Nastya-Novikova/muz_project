/*using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class PortfolioController : ControllerBase
    {
        private readonly IAudioUploadService _audioService;
        private readonly IVideoUploadService _videoService;
        private readonly IPhotoUploadService _photoService;
        private readonly IPortfolioService _portfolioService;
        private readonly IProfileService _profileService;

        public PortfolioController(
            IAudioUploadService audioService,
            IVideoUploadService videoService,
            IPhotoUploadService photoService,
            IPortfolioService portfolioService,
            IProfileService profileService)
        {
            _audioService = audioService;
            _videoService = videoService;
            _photoService = photoService;
            _portfolioService = portfolioService;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPortfolio()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return BadRequest();
            var profileId = Guid.Parse(profile.RootElement.GetProperty("id").GetString()!);
            var result = await _portfolioService.GetPortfolioAsync(profileId);
            return result != null ? Ok(result) : BadRequest();
        }

        [HttpPost("audio")]
        public async Task<IActionResult> UploadAudio(IFormFile file, [FromForm] string title, [FromForm] string? description = null)
        {
            var userId = GetUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return BadRequest("Профиль не найден");
            var profileId = Guid.Parse(profile.RootElement.GetProperty("id").GetString()!);
            var result = await _audioService.UploadAudioAsync(profileId, file, title, description);
            return Ok(result);
        }

        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(IFormFile file, [FromForm] string title, [FromForm] string? description = null)
        {
            var userId = GetUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return BadRequest("Профиль не найден");
            var profileId = Guid.Parse(profile.RootElement.GetProperty("id").GetString()!);
            var result = await _videoService.UploadVideoAsync(profileId, file, title, description);
            return Ok(result);
        }

        [HttpPost("photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file, [FromForm] string title, [FromForm] string? description = null)
        {
            var userId = GetUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return BadRequest("Профиль не найден");
            var profileId = Guid.Parse(profile.RootElement.GetProperty("id").GetString()!);
            var result = await _photoService.UploadPhotoAsync(profileId, file, title, description);
            return Ok(result);
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirst("userId")?.Value!);
    }
}
*/