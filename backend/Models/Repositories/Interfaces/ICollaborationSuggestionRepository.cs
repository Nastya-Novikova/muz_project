using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface ICollaborationSuggestionRepository
{
    DbSet<CollaborationSuggestion> Suggestions { get; }
    Task AddAsync(CollaborationSuggestion suggestion);
    Task<List<CollaborationSuggestion>> GetReceivedAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true);
    Task<List<CollaborationSuggestion>> GetSentAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true);
}