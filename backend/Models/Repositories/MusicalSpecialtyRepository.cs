// Аналогично CityRepository
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class MusicalSpecialtyRepository : IMusicalSpecialtyRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<MusicalSpecialty> MusicalSpecialties { get; set; }

    public MusicalSpecialtyRepository(MusicianFinderDbContext context)
    {
        _context = context;
        MusicalSpecialties = _context.Set<MusicalSpecialty>();
    }

    public async Task<List<MusicalSpecialty>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
    {
        var queryable = MusicalSpecialties.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(s =>
                s.Name.Contains(query) ||
                s.LocalizedName.Contains(query));
        }

        queryable = ApplySorting(queryable, sortBy, sortDesc);
        return await queryable.ToListAsync();
    }

    public async Task<MusicalSpecialty?> GetByIdAsync(int id)
    {
        return await MusicalSpecialties.FindAsync(id);
    }

    private static IQueryable<MusicalSpecialty> ApplySorting(IQueryable<MusicalSpecialty> query, string? sortBy, bool sortDesc)
    {
        return sortBy?.ToLower() switch
        {
            "name" => sortDesc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "localizedname" => sortDesc ? query.OrderByDescending(s => s.LocalizedName) : query.OrderBy(s => s.LocalizedName),
            _ => query.OrderByDescending(s => s.Id)
        };
    }
}