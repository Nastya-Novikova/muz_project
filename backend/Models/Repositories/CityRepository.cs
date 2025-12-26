using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class CityRepository : ICityRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<City> Cities { get; set; }

    public CityRepository(MusicianFinderDbContext context)
    {
        _context = context;
        Cities = _context.Set<City>();
    }

    public async Task<List<City>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
    {
        var queryable = Cities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(c =>
                c.Name.Contains(query) ||
                c.LocalizedName.Contains(query));
        }

        queryable = ApplySorting(queryable, sortBy, sortDesc);
        return await queryable.ToListAsync();
    }

    public async Task<City?> GetByIdAsync(int id)
    {
        return await Cities.FindAsync(id);
    }

    private static IQueryable<City> ApplySorting(IQueryable<City> query, string? sortBy, bool sortDesc)
    {
        return sortBy?.ToLower() switch
        {
            "name" => sortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "localizedname" => sortDesc ? query.OrderByDescending(c => c.LocalizedName) : query.OrderBy(c => c.LocalizedName),
            _ => query.OrderByDescending(c => c.Id)
        };
    }
}