using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик события регистрации на мероприятие. Сбрасывает кеш мероприятия и списков пользователя.
    /// </summary>
    public class UserRegisteredToEventEventHandler : INotificationHandler<UserRegisteredToEventDomainEvent>
    {
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserRegisteredToEventEventHandler"/>.
        /// </summary>
        /// <param name="cache">Сервис кеша.</param>
        public UserRegisteredToEventEventHandler(ICacheService cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task Handle(UserRegisteredToEventDomainEvent notification, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync($"event:{notification.EventId}");
            await _cache.RemoveAsync("events:list");
            await _cache.RemoveAsync($"user-events:{notification.ProfileId}");
        }
    }
}