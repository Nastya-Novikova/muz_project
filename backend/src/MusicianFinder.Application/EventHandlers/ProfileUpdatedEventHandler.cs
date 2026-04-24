using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.DomainEvents;

namespace MusicianFinder.Application.EventHandlers
{
    /// <summary>
    /// Обработчик события обновления профиля. Сбрасывает кеш профиля.
    /// </summary>
    public class ProfileUpdatedEventHandler : INotificationHandler<ProfileUpdatedDomainEvent>
    {
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ProfileUpdatedEventHandler"/>.
        /// </summary>
        /// <param name="cache">Сервис кеша.</param>
        public ProfileUpdatedEventHandler(ICacheService cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task Handle(ProfileUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync($"profile:{notification.ProfileId}");
        }
    }
}