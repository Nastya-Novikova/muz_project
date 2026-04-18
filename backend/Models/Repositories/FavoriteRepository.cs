using backend.Data;
using backend.Exceptions;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly MusicianFinderDbContext _context;

        public FavoriteRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public async Task RemoveAsync(Guid userId, Guid profileId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProfileId == profileId);

            _context.Favorites.Remove(favorite);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid profileId)
        {
            return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProfileId == profileId);
        }

        public async Task<List<MusicianProfile>> GetFavoritesByUserIdAsync(Guid userId, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 20;

            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Include(s => s.Profile)
                    .ThenInclude(p => p.City)
                .Include(s => s.Profile)
                    .ThenInclude(p => p.Genres)
                .Include(s => s.Profile)
                    .ThenInclude(p => p.CollaborationGoals)
                .Include(s => s.Profile)
                    .ThenInclude(p => p.Specialties)
                .Include(s => s.Profile)
                    .ThenInclude(p => p.DesiredGenres)
                .Include(s => s.Profile)
                    .ThenInclude(p => p.DesiredSpecialties)
                .Select(f => f.Profile)
                .ToListAsync();
        }

        public async Task<int> CountFavoritesByUserIdAsync(Guid userId)
        {
            return await _context.Favorites.CountAsync(f => f.UserId == userId);
        }
    }
}
