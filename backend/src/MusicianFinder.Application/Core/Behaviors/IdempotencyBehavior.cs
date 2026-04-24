using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MusicianFinder.Application.Core.Behaviors
{
    /// <summary>
    /// Поведение, проверяющее идемпотентность команды. Если команда с таким ключом уже выполнялась,
    /// возвращает сохранённый ответ, предотвращая повторное выполнение.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IdempotencyBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="cache">Распределённый кеш.</param>
        public IdempotencyBehavior(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = $"idempotency:{request.IdempotencyKey}";
            var cachedResult = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return JsonSerializer.Deserialize<TResponse>(cachedResult)!;
            }

            var response = await next();

            var serializedResponse = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            }, cancellationToken);

            return response;
        }
    }
}