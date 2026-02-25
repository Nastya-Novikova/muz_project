using backend.Models.Common;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Common;
using System.Text.Json;

namespace backend.Services.Interfaces;

public interface ICollaborationService
{
    Task<Result> SendSuggestionAsync(Guid fromUserId, Guid toProfileId, string? message);
    Task<Result<PagedResult<SuggestionDto>>> GetReceivedAsync(Guid userId, int page, int limit, string? sortBy, bool sortDesc);
    Task<Result<PagedResult<SuggestionDto>>> GetSentAsync(Guid userId, int page, int limit, string? sortBy, bool sortDesc);
    Task<Result<bool>> IsCollaboratedAsync(Guid userId, Guid collaboratedProfileId);
}