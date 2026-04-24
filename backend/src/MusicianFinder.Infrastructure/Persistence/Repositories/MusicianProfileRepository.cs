using Microsoft.EntityFrameworkCore;
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
            => await _dbContext.MusicianProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct);

        /// <inheritdoc />
        public void Add(MusicianProfile profile) => _dbContext.MusicianProfiles.Add(profile);
    }
}