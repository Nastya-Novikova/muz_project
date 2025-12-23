// Аналогично CityRepository
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<Genre> Genres { get; set; }

    public GenreRepository(MusicianFinderDbContext context)
    {
        _context = context;
        Genres = _context.Set<Genre>();
    }

    public async Task<List<Genre>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
    {
        var queryable = Genres.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(g =>
                g.Name.Contains(query) ||
                g.LocalizedName.Contains(query));
        }

        queryable = ApplySorting(queryable, sortBy, sortDesc);
        return await queryable.ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await Genres.FindAsync(id);
    }

    private static IQueryable<Genre> ApplySorting(IQueryable<Genre> query, string? sortBy, bool sortDesc)
    {
        return sortBy?.ToLower() switch
        {
            "name" => sortDesc ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
            "localizedname" => sortDesc ? query.OrderByDescending(g => g.LocalizedName) : query.OrderBy(g => g.LocalizedName),
            _ => query.OrderByDescending(g => g.Id)
        };
    }
}