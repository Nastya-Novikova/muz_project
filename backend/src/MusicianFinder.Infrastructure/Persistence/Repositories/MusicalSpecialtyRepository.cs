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
    /// Репозиторий для работы со справочником музыкальных специальностей.
    /// </summary>
    public class MusicalSpecialtyRepository : IMusicalSpecialtyRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MusicalSpecialtyRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public MusicalSpecialtyRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<MusicalSpecialty?> GetByIdAsync(int id)
        {
            return await _context.MusicalSpecialties.FindAsync(id);
        }

        /// <inheritdoc />
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
}