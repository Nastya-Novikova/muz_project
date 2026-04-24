using MusicianFinder.Domain.Common;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Диспатчер доменных событий.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Отправляет все события из агрегата на обработку.
        /// </summary>
        /// <param name="aggregateRoot">Корень агрегата.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task DispatchAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken);
    }
}