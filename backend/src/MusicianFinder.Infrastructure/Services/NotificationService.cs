using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для отправки уведомлений пользователям.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IVkService _vkService;
        private readonly IMusicianProfileRepository _profileRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotificationService"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="emailService">Сервис email.</param>
        /// <param name="vkService">Сервис VK.</param>
        public NotificationService(
            AppDbContext dbContext,
            IEmailService emailService,
            IVkService vkService,
            IMusicianProfileRepository musicianProfileRepository)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _vkService = vkService;
            _profileRepository = musicianProfileRepository;
        }

        /// <inheritdoc />
        public async Task SendNotificationToProfileAsync(MusicianProfile profile, NotificationType type, Dictionary<string, object> data)
        {
            var (title, message) = GetNotificationText(type, data);
            var notification = new Notification(
                profile.Id,
                type,
                title,
                GetEntityType(type),
                GetEntityId(data),
                message);

            await _profileRepository.AddNotificationAsync(profile.Id, notification);

            //var profile = await _profileRepository.

            // Email/VK можно отправить, прочитав настройки из того же профиля (он в памяти)
            if (profile.NotifyByEmail && !string.IsNullOrEmpty(profile.Email))
                await _emailService.SendNotificationAsync(profile.Email, title, message);
            if (profile.NotifyByVk && profile.VkUserId != null)
                await _vkService.SendNotificationAsync(profile.UserId, message ?? title);
        }

        /// <inheritdoc />
        public async Task SendNotificationToUserAsync(Guid userId, NotificationType type, Dictionary<string, object> data)
        {
            var profile = await _dbContext.MusicianProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) return;

            await SendNotificationToProfileAsync(profile, type, data);
        }

        private static (string Title, string Message) GetNotificationText(
            NotificationType type, Dictionary<string, object> data)
        {
            return type switch
            {
                NotificationType.CollaborationReceived => (
                    $"Пользователь {data["fromProfileName"]} отправил вам предложение о сотрудничестве",
                    data.TryGetValue("message", out var msg)
                        ? msg?.ToString() ?? "У вас новое предложение о сотрудничестве"
                        : "У вас новое предложение о сотрудничестве"
                ),
                NotificationType.EventRegistration => (
                    "Регистрация подтверждена",
                    $"Вы успешно зарегистрировались на мероприятие \"{data["eventTitle"]}\""
                ),
                NotificationType.EventReminder => (
                    $"Через {data["daysLeft"]} дн. состоится мероприятие \"{data["eventTitle"]}\"",
                    "Не забудьте о предстоящем мероприятии"
                ),
                _ => ("Новое уведомление", string.Empty)
            };
        }

        private static EntityType GetEntityType(NotificationType type)
        {
            return type switch
            {
                NotificationType.CollaborationReceived => EntityType.CollaborationSuggestion,
                NotificationType.EventRegistration => EntityType.Event,
                NotificationType.EventReminder => EntityType.Event,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static Guid GetEntityId(Dictionary<string, object> data)
        {
            return data.TryGetValue("suggestionId", out var sid) ? (Guid)sid
                 : data.TryGetValue("eventId", out var eid) ? (Guid)eid
                 : Guid.Empty;
        }
    }
}