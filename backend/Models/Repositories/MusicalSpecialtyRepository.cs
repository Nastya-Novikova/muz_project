using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class MusicalSpecialtyRepository : IMusicalSpecialtyRepository
{
    private readonly MusicianFinderDbContext _context;
    //public DbSet<MusicalSpecialty> MusicalSpecialties { get; set; }

    public MusicalSpecialtyRepository(MusicianFinderDbContext context)
    {
        _context = context;
        //MusicalSpecialties = _context.Set<MusicalSpecialty>();
    }

    public async Task<List<MusicalSpecialty>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
    {
        var queryable = _context.MusicalSpecialties.AsQueryable();

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
        return await _context.MusicalSpecialties.FindAsync(id);
    }

    public async Task<List<MusicalSpecialty>> GetByIdsAsync(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return new List<MusicalSpecialty>();

        return await _context.MusicalSpecialties.Where(s => ids.Contains(s.Id)).ToListAsync();
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