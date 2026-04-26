using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация репозитория для записи музыкальных профилей.
    /// </summary>
    public class MusicianProfileRepository : IMusicianProfileRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MusicianProfileRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public MusicianProfileRepository(AppDbContext dbContext) => _dbContext = dbContext;

        /// <inheritdoc />
        public async Task<MusicianProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
            => await _dbContext.MusicianProfiles.IgnoreAutoIncludes().Include(p => p.Portfolio).Include(p => p.Favorites).FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct);

        /// <inheritdoc />
        public void Add(MusicianProfile profile) => _dbContext.MusicianProfiles.Add(profile);

        public async Task AddPortfolioItemAsync(Guid userId, PortfolioItem item, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct)
                ?? throw new NotFoundException("Профиль не найден.");

            profile.AddPortfolioItem(item);
            // При необходимости явно указать состояние (обычно не требуется после IgnoreAutoIncludes)
            _dbContext.Entry(item).State = EntityState.Added;
        }

        public async Task AddFavoriteAsync(Guid userId, Guid targetProfileId, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct)
                ?? throw new NotFoundException("Профиль не найден.");

            profile.AddToFavorites(targetProfileId); // здесь внутри создаётся new Favorite

            // Найти свеже-добавленный объект Favorite
            var favorite = _dbContext.Entry(profile)
                .Collection(p => p.Favorites)
                .CurrentValue?
                .LastOrDefault();

            if (favorite != null)
                _dbContext.Entry(favorite).State = EntityState.Added;
        }

        public async Task AddNotificationToProfileAsync(Guid profileId, Notification notification, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .Include(p => p.Notifications)
                .FirstOrDefaultAsync(p => p.Id == profileId && !p.IsDeleted, ct)
                ?? throw new NotFoundException("Профиль не найден.");

            profile.AddNotification(notification);

            // Явно указываем, что это новый owned-объект
            var added = _dbContext.Entry(profile)
                .Collection(p => p.Notifications)
                .CurrentValue?
                .LastOrDefault();

            if (added != null)
                _dbContext.Entry(added).State = EntityState.Added;
        }

        public async Task<MusicianProfile?> GetByUserIdWithNotificationsAsync(Guid userId, CancellationToken ct = default)
            => await _dbContext.MusicianProfiles
                .Include(p => p.Notifications)
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct);

        public async Task<MusicianProfile?> GetByIdWithNotificationsAsync(Guid profileId, CancellationToken ct = default)
            => await _dbContext.MusicianProfiles
                .Include(p => p.Notifications)
                .FirstOrDefaultAsync(p => p.Id == profileId && !p.IsDeleted, ct);

        public async Task AddNotificationAsync(Guid profileId, Notification notification, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .Include(p => p.Notifications)
                .FirstOrDefaultAsync(p => p.Id == profileId && !p.IsDeleted, ct)
                ?? throw new NotFoundException("Профиль не найден.");

            profile.AddNotification(notification);

            var added = _dbContext.Entry(profile)
                .Collection(p => p.Notifications)
                .CurrentValue?
                .LastOrDefault();
            if (added != null)
                _dbContext.Entry(added).State = EntityState.Added;
        }
    }
}