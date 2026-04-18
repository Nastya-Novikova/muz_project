using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class CollaborationSuggestionRepository : ICollaborationSuggestionRepository
{
    private readonly MusicianFinderDbContext _context;

    public CollaborationSuggestionRepository(MusicianFinderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CollaborationSuggestion suggestion)
    {
        await _context.CollaborationSuggestions.AddAsync(suggestion);
    }

    public async Task<List<CollaborationSuggestion>> GetReceivedAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true)
    {
        var query = _context.CollaborationSuggestions
            .Include(s => s.FromProfile)
                .ThenInclude(p => p.City)
            .Include(s => s.FromProfile)
                .ThenInclude(p => p.Genres)
            .Include(s => s.FromProfile)
                .ThenInclude(p => p.CollaborationGoals)
            .Include(s => s.FromProfile)
                .ThenInclude(p => p.Specialties)
            .Include(s => s.FromProfile)
                .ThenInclude(p => p.DesiredGenres)
            .Include(s => s.FromProfile)
                .ThenInclude(p => p.DesiredSpecialties)
            .Where(s => s.ToProfileId == userId);

        query = ApplySorting(query, sortBy, sortDesc);
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<CollaborationSuggestion>> GetSentAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true)
    {
        var query = _context.CollaborationSuggestions
            .Include(s => s.ToProfile)
                .ThenInclude(p => p.City)
            .Include(s => s.ToProfile)
                .ThenInclude(p => p.Genres)
            .Include(s => s.ToProfile)
                .ThenInclude(p => p.CollaborationGoals)
            .Include(s => s.ToProfile)
                .ThenInclude(p => p.Specialties)
            .Include(s => s.ToProfile)
                .ThenInclude(p => p.DesiredGenres)
            .Include(s => s.ToProfile)
                .ThenInclude(p => p.DesiredSpecialties)
            .Where(s => s.FromProfileId == userId);

        query = ApplySorting(query, sortBy, sortDesc);
        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<CollaborationSuggestion?> GetByIdAsync(Guid id)
    {
        return await _context.CollaborationSuggestions
            .Include(s => s.FromProfile)
            .Include(s => s.ToProfile)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateAsync(CollaborationSuggestion suggestion)
    {
        _context.CollaborationSuggestions.Update(suggestion);
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