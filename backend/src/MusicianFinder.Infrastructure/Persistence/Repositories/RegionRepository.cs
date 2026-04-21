using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы со справочником регионов.
    /// </summary>
    public class RegionRepository : IRegionRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RegionRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public RegionRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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