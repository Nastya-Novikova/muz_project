using System.Text.Json;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;

namespace backend.Services;

public class CollaborationService : ICollaborationService
{
    private readonly ICollaborationSuggestionRepository _suggestionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;

    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public CollaborationService(
        ICollaborationSuggestionRepository suggestionRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository)
    {
        _suggestionRepository = suggestionRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    public async Task<JsonDocument?> SendSuggestionAsync(Guid userId, Guid toProfileId, string? message)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return null;
            var fromProfile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            var toProfile = await _profileRepository.GetByIdAsync(toProfileId);
            if (fromProfile == null || toProfile == null) return null;

            var suggestion = new CollaborationSuggestion
            {
                Id = Guid.NewGuid(),
                FromProfileId = fromProfile.Id,
                ToProfileId = toProfile.Id,
                Message = message
            };

            await _suggestionRepository.AddAsync(suggestion);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true, collaborationId = suggestion.Id }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetReceivedAsync(Guid userId, JsonDocument queryParams)
    {
        try
        {
            var root = queryParams.RootElement;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;
            var sortBy = root.TryGetProperty("sortBy", out var sb) ? sb.GetString() : "createdAt";
            var sortDesc = root.TryGetProperty("sortDesc", out var sd) ? sd.GetBoolean() : true;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return null;
            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (profile == null) return null;
            var result = await _suggestionRepository.GetReceivedAsync(profile.Id, page, limit, sortBy, sortDesc);
            var suggestions = result.Select(async suggestion =>
            {
                if (suggestion == null) return null;
                var profile = await _profileRepository.GetByIdAsync(suggestion.FromProfileId);
                if (profile == null) return null;
                return new
                {
                    FromProfile = new {
                    profile.Id,
                    profile.FullName,
                    profile.Description,
                    profile.Phone,
                    profile.Telegram,
                    profile.City,
                    profile.Experience,
                    profile.Age,
                    profile.Avatar,
                    Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                    Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                    CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g))
                    },
                    suggestion.Message,
                    suggestion.Status,
                    suggestion.CreatedAt
                };
            });
            return JsonDocument.Parse(JsonSerializer.Serialize(suggestions, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetSentAsync(Guid userId, JsonDocument queryParams)
    {
        try
        {
            var root = queryParams.RootElement;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;
            var sortBy = root.TryGetProperty("sortBy", out var sb) ? sb.GetString() : "createdAt";
            var sortDesc = root.TryGetProperty("sortDesc", out var sd) ? sd.GetBoolean() : true;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.MusicianProfile == null) return null;
            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (profile == null) return null;
            var result = await _suggestionRepository.GetSentAsync(profile.Id, page, limit, sortBy, sortDesc);
            var suggestions = result.Select(async suggestion =>
            {
                if (suggestion == null) return null;
                var profile = await _profileRepository.GetByIdAsync(suggestion.ToProfileId);
                if (profile == null) return null;
                return new
                {
                    ToProfile = new
                    {
                        profile.Id,
                        profile.FullName,
                        profile.Description,
                        profile.Phone,
                        profile.Telegram,
                        profile.City,
                        profile.Experience,
                        profile.Age,
                        profile.Avatar,
                        Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                        Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                        CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g))
                    },
                    suggestion.Message,
                    suggestion.Status,
                    suggestion.CreatedAt
                };
            });
            return JsonDocument.Parse(JsonSerializer.Serialize(suggestions, _options));
        }
        catch
        {
            return null;
        }
    }
}