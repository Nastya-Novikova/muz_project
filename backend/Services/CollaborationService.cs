using System.Text.Json;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

public class CollaborationService : ICollaborationService
{
    private readonly ICollaborationSuggestionRepository _suggestionRepository;
    private readonly IUserRepository _userRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public CollaborationService(
        ICollaborationSuggestionRepository suggestionRepository,
        IUserRepository userRepository)
    {
        _suggestionRepository = suggestionRepository;
        _userRepository = userRepository;
    }

    public async Task<JsonDocument?> SendSuggestionAsync(Guid fromUserId, Guid toUserId, string? message)
    {
        try
        {
            var fromUser = await _userRepository.GetByIdAsync(fromUserId);
            var toUser = await _userRepository.GetByIdAsync(toUserId);
            if (fromUser == null || toUser == null) return null;

            var suggestion = new CollaborationSuggestion
            {
                Id = Guid.NewGuid(),
                FromUserId = fromUserId,
                ToUserId = toUserId,
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

            var suggestions = await _suggestionRepository.GetReceivedAsync(userId, page, limit, sortBy, sortDesc);
            var result = new { suggestions };
            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
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

            var suggestions = await _suggestionRepository.GetSentAsync(userId, page, limit, sortBy, sortDesc);
            var result = new { suggestions };
            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
        }
        catch
        {
            return null;
        }
    }
}