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
    /// Репозиторий для работы со справочником целей сотрудничества.
    /// </summary>
    public class CollaborationGoalRepository : ICollaborationGoalRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CollaborationGoalRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public CollaborationGoalRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<List<CollaborationGoal>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
        {
            var queryable = _context.CollaborationGoals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(g =>
                    g.Name.Contains(query) ||
                    g.LocalizedName.Contains(query));
            }

            queryable = ApplySorting(queryable, sortBy, sortDesc);
            return await queryable.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<CollaborationGoal?> GetByIdAsync(int id)
        {
            return await _context.CollaborationGoals.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task<List<CollaborationGoal>> GetByIdsAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<CollaborationGoal>();

            return await _context.CollaborationGoals.Where(g => ids.Contains(g.Id)).ToListAsync();
        }

        private static IQueryable<CollaborationGoal> ApplySorting(IQueryable<CollaborationGoal> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => sortDesc ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
                "localizedname" => sortDesc ? query.OrderByDescending(g => g.LocalizedName) : query.OrderBy(g => g.LocalizedName),
                _ => query.OrderByDescending(g => g.Id)
            };
        }
    }
}