using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class CollaborationSuggestionRepository : ICollaborationSuggestionRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<CollaborationSuggestion> Suggestions { get; set; }

    public CollaborationSuggestionRepository(MusicianFinderDbContext context)
    {
        _context = context;
        Suggestions = _context.Set<CollaborationSuggestion>();
    }

    public async Task AddAsync(CollaborationSuggestion suggestion)
    {
        await Suggestions.AddAsync(suggestion);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CollaborationSuggestion>> GetReceivedAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true)
    {
        var query = Suggestions
            .Include(s => s.FromUser)
            .Where(s => s.ToUserId == userId);

        query = ApplySorting(query, sortBy, sortDesc);
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<CollaborationSuggestion>> GetSentAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true)
    {
        var query = Suggestions
            .Include(s => s.ToUser)
            .Where(s => s.FromUserId == userId);

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