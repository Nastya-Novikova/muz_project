using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Реализация сервиса кеша на основе Redis.
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RedisCacheService"/>.
        /// </summary>
        /// <param name="cache">Распределённый кеш.</param>
        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var json = await _cache.GetStringAsync(key);
            return json == null ? null : JsonSerializer.Deserialize<T>(json);
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            var json = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json, options);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}