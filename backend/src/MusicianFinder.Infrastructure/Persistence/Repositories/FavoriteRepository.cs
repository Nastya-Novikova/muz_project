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
    /// Репозиторий для работы с избранными профилями.
    /// </summary>
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FavoriteRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public FavoriteRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(Guid userId, Guid profileId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProfileId == profileId);
            if (favorite != null)
                _context.Favorites.Remove(favorite);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(Guid userId, Guid profileId)
        {
            return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProfileId == profileId);
        }

        /// <inheritdoc />
        public async Task<List<MusicianProfile>> GetFavoritesByUserIdAsync(Guid userId, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 20;

            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Include(f => f.Profile)
                    .ThenInclude(p => p.City)
                .Include(f => f.Profile)
                    .ThenInclude(p => p.Genres)
                .Include(f => f.Profile)
                    .ThenInclude(p => p.CollaborationGoals)
                .Include(f => f.Profile)
                    .ThenInclude(p => p.Specialties)
                .Include(f => f.Profile)
                    .ThenInclude(p => p.DesiredGenres)
                .Include(f => f.Profile)
                    .ThenInclude(p => p.DesiredSpecialties)
                .Select(f => f.Profile)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<int> CountFavoritesByUserIdAsync(Guid userId)
        {
            return await _context.Favorites.CountAsync(f => f.UserId == userId);
        }
    }
}