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
        //public DbSet<Favorite> Favorites { get; set; }

        public FavoriteRepository(MusicianFinderDbContext context)
        {
            _context = context;
          //Favorites = _context.Set<Favorite>();
        }

        public async Task AddAsync(Favorite favorite)
        {
            if (favorite.UserId == Guid.Empty || favorite.ProfileId == Guid.Empty)
                throw new ApiException(400, "ID пользователя и профиля обязательны", "MISSING_IDS");

            await _context.Favorites.AddAsync(favorite);
        }

        public async Task RemoveAsync(Guid userId, Guid profileId)
        {
            if (userId == Guid.Empty || profileId == Guid.Empty)
                throw new ApiException(400, "ID пользователя и профиля обязательны", "MISSING_IDS");

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProfileId == profileId);

            if (favorite == null)
                throw new ApiException(404, "Избранное не найдено", "FAVORITE_NOT_FOUND");

            _context.Favorites.Remove(favorite);
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid profileId)
        {
            if (userId == Guid.Empty || profileId == Guid.Empty)
                throw new ApiException(400, "ID пользователя и профиля обязательны", "MISSING_IDS");

            return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProfileId == profileId);
        }

        public async Task<List<MusicianProfile>> GetFavoritesByUserIdAsync(Guid userId, int page, int limit)
        {
            if (userId == Guid.Empty)
                throw new ApiException(400, "ID пользователя не может быть пустым", "INVALID_USER_ID");

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
                    .ThenInclude(p => p.Specialties)
                .Select(f => f.Profile)
                .ToListAsync();
        }

        public async Task<int> CountFavoritesByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ApiException(400, "ID пользователя не может быть пустым", "INVALID_USER_ID");

            return await _context.Favorites.CountAsync(f => f.UserId == userId);
        }
    }
}
