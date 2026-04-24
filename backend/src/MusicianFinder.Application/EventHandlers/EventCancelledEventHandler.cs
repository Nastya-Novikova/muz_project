using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик события отмены мероприятия. Сбрасывает кеш мероприятия и общего списка.
    /// </summary>
    public class EventCancelledEventHandler : INotificationHandler<EventCancelledDomainEvent>
    {
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventCancelledEventHandler"/>.
        /// </summary>
        /// <param name="cache">Сервис кеша.</param>
        public EventCancelledEventHandler(ICacheService cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task Handle(EventCancelledDomainEvent notification, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync($"event:{notification.EventId}");
            await _cache.RemoveAsync("events:list");
        }
    }
}