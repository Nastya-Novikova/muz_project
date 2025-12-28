using System.Text.Json;

namespace backend.Services.Interfaces;

public interface ICollaborationService
{
    Task<JsonDocument?> SendSuggestionAsync(Guid fromUserId, Guid toUserId, string? message);
    Task<JsonDocument?> GetReceivedAsync(Guid userId, JsonDocument queryParams);
    Task<JsonDocument?> GetSentAsync(Guid userId, JsonDocument queryParams);
    Task<bool> IsCollaboratedAsync(Guid userId, Guid collaboratedUserId);
}