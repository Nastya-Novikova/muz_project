using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для отправки уведомлений пользователям.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IVkService _vkService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="NotificationService"/>.
        /// </summary>
        /// <param name="notificationRepository">Репозиторий уведомлений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="emailService">Сервис email.</param>
        /// <param name="vkService">Сервис VK.</param>
        public NotificationService(
            INotificationRepository notificationRepository,
            IProfileRepository profileRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IVkService vkService)
        {
            _notificationRepository = notificationRepository;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _vkService = vkService;
        }

        /// <inheritdoc />
        public async Task SendNotificationToProfileAsync(Guid profileId, NotificationType type, Dictionary<string, object> data)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId);
            if (profile == null)
                return;

            var (title, message) = GetNotificationText(type, data);

            var internalNotification = new Notification(
                profileId,
                type,
                title,
                GetEntityType(type),
                GetEntityId(data),
                message);

            await _notificationRepository.AddAsync(internalNotification);

            if (profile.NotifyByEmail && !string.IsNullOrEmpty(profile.Email))
                await _emailService.SendNotificationAsync(profile.Email, title, message);

            if (profile.NotifyByVk && !string.IsNullOrEmpty(profile.VkUserId))
            {
                var user = await _userRepository.GetByMusicianProfileIdAsync(profile.Id);
                if (user != null)
                    await _vkService.SendNotificationAsync(user.Id, message);
            }
        }

        /// <inheritdoc />
        public async Task SendNotificationToUserAsync(Guid userId, NotificationType type, Dictionary<string, object> data)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return;

            await SendNotificationToProfileAsync(user.MusicianProfile.Id, type, data);
        }

        private static (string Title, string Message) GetNotificationText(NotificationType type, Dictionary<string, object> data)
        {
            return type switch
            {
                NotificationType.CollaborationReceived => (
                    $"Пользователь {data["fromProfileName"]} отправил вам предложение о сотрудничестве",
                    data.TryGetValue("message", out var msg) ? msg?.ToString() ?? "У вас новое предложение о сотрудничестве" : "У вас новое предложение о сотрудничестве"
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