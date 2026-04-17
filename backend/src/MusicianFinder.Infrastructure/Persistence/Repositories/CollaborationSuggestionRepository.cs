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
    /// Репозиторий для работы с предложениями о сотрудничестве.
    /// </summary>
    public class CollaborationSuggestionRepository : ICollaborationSuggestionRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CollaborationSuggestionRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public CollaborationSuggestionRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(CollaborationSuggestion suggestion)
        {
            await _context.CollaborationSuggestions.AddAsync(suggestion);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<CollaborationSuggestion?> GetByIdAsync(Guid id)
        {
            return await _context.CollaborationSuggestions
                .Include(s => s.FromProfile)
                .Include(s => s.ToProfile)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(CollaborationSuggestion suggestion)
        {
            _context.CollaborationSuggestions.Update(suggestion);
            await Task.CompletedTask;
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
}