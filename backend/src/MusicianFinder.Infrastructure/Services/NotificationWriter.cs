using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Реализация <see cref="INotificationWriter"/>, делегирующая сохранение репозиторию профилей.
    /// </summary>
    public class NotificationWriter : INotificationWriter
    {
        private readonly IMusicianProfileRepository _profileRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotificationWriter"/>.
        /// </summary>
        public NotificationWriter(IMusicianProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task AddAsync(Guid profileId, Notification notification, CancellationToken ct = default)
        {
            await _profileRepository.AddNotificationAsync(profileId, notification, ct);
        }
    }
}