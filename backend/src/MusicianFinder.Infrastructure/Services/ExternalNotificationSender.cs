using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Helpers;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Временная реализация <see cref="IExternalNotificationSender"/>.
    /// Отправляет email и VK‑сообщения напрямую, без внешней шины.
    /// </summary>
    public class ExternalNotificationSender : IExternalNotificationSender
    {
        private readonly IEmailService _email;
        private readonly IVkService _vk;
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ExternalNotificationSender"/>.
        /// </summary>
        public ExternalNotificationSender(IEmailService email, IVkService vk, AppDbContext dbContext)
        {
            _email = email;
            _vk = vk;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task SendAsync(Guid profileId, NotificationType type, IReadOnlyDictionary<string, object> data, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == profileId && !p.IsDeleted, ct);

            if (profile == null) return;

            var (title, message) = NotificationTextBuilder.Build(type, data);

            if (profile.NotifyByEmail && !string.IsNullOrEmpty(profile.Email))
                await _email.SendNotificationAsync(profile.Email, title, message);

            if (profile.NotifyByVk && profile.VkUserId != null)
                await _vk.SendNotificationAsync(profile.UserId, message ?? title);
        }
    }
}