using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories
{
    public class RegionRepository(MusicianFinderDbContext context) : IRegionRepository
    {
        private readonly MusicianFinderDbContext _context = context;

        public async Task<List<Region>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
        {
            var queryable = _context.Regions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(r =>
                    r.Name.Contains(query) ||
                    r.LocalizedName.Contains(query));
            }

            queryable = ApplySorting(queryable, sortBy, sortDesc);
            return await queryable.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(int id)
        {
            return await _context.Regions.FindAsync(id);
        }

        private static IQueryable<Region> ApplySorting(IQueryable<Region> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => sortDesc ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                "localizedname" => sortDesc ? query.OrderByDescending(r => r.LocalizedName) : query.OrderBy(r => r.LocalizedName),
                _ => query.OrderByDescending(r => r.Id)
            };
        }
    }
}
