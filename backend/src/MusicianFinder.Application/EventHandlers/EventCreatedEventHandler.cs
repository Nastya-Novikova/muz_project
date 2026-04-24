using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик события создания мероприятия. Сбрасывает список мероприятий.
    /// </summary>
    public class EventCreatedEventHandler : INotificationHandler<EventCreatedDomainEvent>
    {
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventCreatedEventHandler"/>.
        /// </summary>
        /// <param name="cache">Сервис кеша.</param>
        public EventCreatedEventHandler(ICacheService cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task Handle(EventCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync("events:list");
        }
    }
}