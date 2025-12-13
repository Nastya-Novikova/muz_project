using backend.Models.Repositories;
using backend.Models;
using System.Text.Json;

namespace backend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<string> SearchAsync(string? city, string? genre)
        {
            var profiles = await _profileRepository.SearchAsync(city, genre);
            return JsonSerializer.Serialize(profiles, _options);
        }

        public async Task<string?> GetByIdAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            return profile == null ? null : JsonSerializer.Serialize(profile, _options);
        }

        public async Task<string> CreateAsync(JsonDocument doc)
        {
            /*using var doc = JsonDocument.Parse(json);*/
            var root = doc.RootElement;

            if (string.IsNullOrWhiteSpace(root.GetProperty("fullName").GetString()))
                throw new ArgumentException("Field 'fullName' is required.");

            var profile = new MusicianProfile
            {
                Id = Guid.NewGuid(),
                FullName = root.GetProperty("fullName").GetString() ?? "",
                City = root.GetProperty("city").GetString() ?? "",
                Bio = root.GetProperty("bio").GetString() ?? "",
                LookingFor = root.GetProperty("lookingFor").GetString() ?? "collaboration",
                Instruments = root.TryGetProperty("instruments", out var instElem)
                    ? instElem.EnumerateArray().Select(x => x.GetString()!).Where(x => x != null).ToList()
                    : new(),
                Genres = root.TryGetProperty("genres", out var genElem)
                    ? genElem.EnumerateArray().Select(x => x.GetString()!).Where(x => x != null).ToList()
                    : new()
            };

            await _profileRepository.AddAsync(profile);
            return JsonSerializer.Serialize(profile, _options);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _profileRepository.GetByIdAsync(id);
            if (existing == null) return false;
            await _profileRepository.DeleteAsync(id);
            return true;
        }
    }
}
