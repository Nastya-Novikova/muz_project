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
    /// Репозиторий для работы со справочником городов.
    /// </summary>
    public class CityRepository : ICityRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CityRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public CityRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<List<City>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
        {
            var queryable = _context.Cities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(c =>
                    c.Name.Contains(query) ||
                    c.LocalizedName.Contains(query));
            }

            queryable = ApplySorting(queryable, sortBy, sortDesc);
            return await queryable.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
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
}