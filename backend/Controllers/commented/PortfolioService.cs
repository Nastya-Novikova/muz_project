/*using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using System.Text.Json;

namespace backend.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioAudioRepository _audioRepo;
        private readonly IPortfolioVideoRepository _videoRepo;
        private readonly IPortfolioPhotoRepository _photoRepo;
        private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public PortfolioService(IPortfolioAudioRepository audioRepo,
            IPortfolioVideoRepository videoRepo,
            IPortfolioPhotoRepository photoRepo)
        {
            _audioRepo = audioRepo;
            _videoRepo = videoRepo;
            _photoRepo = photoRepo;
        }

        public async Task<JsonDocument?> GetPortfolioAsync(Guid profileId)
        {
            try
            {
                var portfolio = new Portfolio
                {
                    Audio = await _audioRepo.GetByProfileIdAsync(profileId),
                    Video = await _videoRepo.GetByProfileIdAsync(profileId),
                    Photos = await _photoRepo.GetByProfileIdAsync(profileId)
                };
                return JsonDocument.Parse(JsonSerializer.Serialize(portfolio, _options));
            }
            catch
            {
                return null;
            }
        }
    }
}
*/