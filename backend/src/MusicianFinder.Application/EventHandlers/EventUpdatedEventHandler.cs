using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик события обновления мероприятия. Сбрасывает кеш мероприятия и общего списка.
    /// </summary>
    public class EventUpdatedEventHandler : INotificationHandler<EventUpdatedDomainEvent>
    {
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventUpdatedEventHandler"/>.
        /// </summary>
        /// <param name="cache">Сервис кеша.</param>
        public EventUpdatedEventHandler(ICacheService cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task Handle(EventUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync($"event:{notification.EventId}");
            await _cache.RemoveAsync("events:list");
        }
    }
}