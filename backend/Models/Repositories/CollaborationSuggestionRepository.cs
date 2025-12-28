using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class CollaborationSuggestionRepository : ICollaborationSuggestionRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<CollaborationSuggestion> Suggestions { get; set; }

    private readonly IProfileRepository _profileRepository;

    public CollaborationSuggestionRepository(MusicianFinderDbContext context, IProfileRepository profileRepository)
    {
        _context = context;
        Suggestions = _context.Set<CollaborationSuggestion>();
        _profileRepository = profileRepository;
    }

    public async Task AddAsync(CollaborationSuggestion suggestion)
    {
        if (suggestion.FromProfileId == Guid.Empty || suggestion.ToProfileId == Guid.Empty)
            throw new ApiException(400, "UserID отправителя и получателя обязательны", "MISSING_USER_IDS");

        if (suggestion.FromProfileId == suggestion.ToProfileId)
            throw new ApiException(400, "Нельзя отправить предложение самому себе", "SELF_SUGGESTION");

        var fromUser = await _profileRepository.GetByIdAsync(suggestion.FromProfileId);
        var toUser = await _profileRepository.GetByIdAsync(suggestion.ToProfileId);

        if (fromUser == null)
            throw new ApiException(404, "Отправитель не найден", "FROM_USER_NOT_FOUND");
        if (toUser == null)
            throw new ApiException(404, "Получатель не найден", "TO_USER_NOT_FOUND");

        await Suggestions.AddAsync(suggestion);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CollaborationSuggestion>> GetReceivedAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true)
    {
        var query = Suggestions
            .Include(s => s.FromProfile)
            .Where(s => s.ToProfileId == userId);

        query = ApplySorting(query, sortBy, sortDesc);
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<CollaborationSuggestion>> GetSentAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true)
    {
        var query = Suggestions
            .Include(s => s.ToProfile)
            .Where(s => s.FromProfileId == userId);

        query = ApplySorting(query, sortBy, sortDesc);
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    private static IQueryable<CollaborationSuggestion> ApplySorting(IQueryable<CollaborationSuggestion> query, string? sortBy, bool sortDesc)
    {
        return sortBy?.ToLower() switch
        {
            "status" => sortDesc ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            "createdat" => sortDesc ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };
    }
}