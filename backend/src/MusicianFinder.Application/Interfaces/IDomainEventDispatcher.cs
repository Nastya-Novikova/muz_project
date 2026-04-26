using MusicianFinder.SharedKernel;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Диспатчер доменных событий. Отправляет события из агрегата на обработку.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Диспатчит все доменные события из указанного агрегата.
        /// </summary>
        /// <param name="aggregateRoot">Корень агрегата, содержащий события.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task DispatchAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken);
    }
}