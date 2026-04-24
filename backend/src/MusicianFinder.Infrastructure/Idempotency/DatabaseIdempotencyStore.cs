using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Idempotency
{
    /// <summary>
    /// Реализация хранилища идемпотентности на основе базы данных.
    /// </summary>
    public class DatabaseIdempotencyStore : IIdempotencyStore
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="DatabaseIdempotencyStore"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public DatabaseIdempotencyStore(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<(bool Created, Application.Interfaces.IdempotencyRecord? Record)> TryCreateAsync(string key, string requestHash)
        {
            var existing = await _dbContext.Set<IdempotencyRecord>().AsNoTracking()
                .FirstOrDefaultAsync(r => r.Key == key);

            if (existing != null)
            {
                return (false, new Application.Interfaces.IdempotencyRecord
                {
                    Key = existing.Key,
                    RequestHash = existing.RequestHash,
                    Response = existing.Response,
                    Status = existing.Status
                });
            }

            var record = new IdempotencyRecord
            {
                Key = key,
                RequestHash = requestHash,
                Status = "InProgress",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Set<IdempotencyRecord>().Add(record);
            await _dbContext.SaveChangesAsync();

            return (true, null);
        }

        /// <inheritdoc />
        public async Task<Application.Interfaces.IdempotencyRecord?> GetAsync(string key)
        {
            var record = await _dbContext.Set<IdempotencyRecord>().AsNoTracking()
                .FirstOrDefaultAsync(r => r.Key == key);

            if (record == null) return null;

            return new Application.Interfaces.IdempotencyRecord
            {
                Key = record.Key,
                RequestHash = record.RequestHash,
                Response = record.Response,
                Status = record.Status
            };
        }

        /// <inheritdoc />
        public async Task UpdateAsync(string key, string response, string status)
        {
            var record = await _dbContext.Set<IdempotencyRecord>().FirstOrDefaultAsync(r => r.Key == key);
            if (record == null) return;

            record.Response = response;
            record.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}