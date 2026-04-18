using AutoMapper;
using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Notifications;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Models.Classes;
using backend.Services.Utils;
using backend.Models.DTOs.Events;

namespace backend.Services
{
    /// <summary>
    /// Сервис для работы с уведомлениями
    /// </summary>
    public class NotificationService(
        INotificationRepository notificationRepository,
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEmailService emailService,
        IVkService vkService,
        IEntityExistenceService existenceService) : INotificationService
    {
        private readonly INotificationRepository _notificationRepository = notificationRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailService _emailService = emailService;
        private readonly IVkService _vkService = vkService;
        private readonly IEntityExistenceService _existenceService = existenceService;

        public async Task SendNotificationToProfileAsync(Guid profileId, NotificationType type, Dictionary<string, object> data)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId);
            if (profile == null) return;

            // Формируем текст в зависимости от типа
            var (title, message) = GetNotificationText(type, data);

            // 1. Внутреннее уведомление (в БД)
            var internalNotification = new Notification
            {
                Id = Guid.NewGuid(),
                ProfileId = profileId,
                Type = type,
                Title = title,
                Message = message,
                EntityType = GetEntityType(type),
                EntityId = GetEntityId(data),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationRepository.AddAsync(internalNotification);

            // 2. Email, если разрешено
            if (profile.NotifyByEmail && !string.IsNullOrEmpty(profile.Email))
            {
                await _emailService.SendNotificationAsync(profile.Email, title, message);
            }

            // 3. VK, если разрешено и привязан
            if (profile.NotifyByVk && !string.IsNullOrEmpty(profile.VkUserId))
            {
                var user = await _userRepository.GetByMusicianProfileIdAsync(profile.Id);
                if (user != null)
                {
                    await _vkService.SendNotificationAsync(user.Id, message);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendNotificationToUserAsync(Guid userId, NotificationType type, Dictionary<string, object> data)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null) return;

            await SendNotificationToProfileAsync(user.MusicianProfile.Id, type, data);
        }

        private static (string Title, string Message) GetNotificationText(NotificationType type, Dictionary<string, object> data)
        {
            switch (type)
            {
                case NotificationType.CollaborationReceived:
                    var message = data.TryGetValue("message", out var msg) ? msg?.ToString() : null;
                    return NotificationMessageProvider.GetCollaborationReceived(
                        data["fromProfileName"].ToString()!,
                        message);

                case NotificationType.EventRegistration:
                    return NotificationMessageProvider.GetEventRegistration(
                        data["eventTitle"].ToString()!);

                case NotificationType.EventReminder:
                    return NotificationMessageProvider.GetEventReminder(
                        data["eventTitle"].ToString()!,
                        (int)data["daysLeft"]);

                default:
                    return ("Новое уведомление", "");
            }
        }

        private static EntityType GetEntityType(NotificationType type)
        {
            return type switch
            {
                NotificationType.CollaborationReceived => EntityType.CollaborationSuggestion,
                NotificationType.EventRegistration => EntityType.Event,
                NotificationType.EventReminder => EntityType.Event,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Unsupported notification type: {type}")
            };
        }

        private static Guid GetEntityId(Dictionary<string, object> data)
        {
            return data.TryGetValue("suggestionId", out var sid) ? (Guid)sid
                 : data.TryGetValue("eventId", out var eid) ? (Guid)eid
                 : Guid.Empty;
        }

        public async Task<Result<PagedResult<NotificationDto>>> GetUserNotificationsAsync(Guid userId, int page, int limit)
        {
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result<PagedResult<NotificationDto>>.Failure(userResult.Error);
            var user = userResult.Value;

            var profileId = user.MusicianProfile.Id;
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var (items, totalCount) = await _notificationRepository.GetByProfileIdAsync(profileId, page, limit, thirtyDaysAgo);

            var dtos = _mapper.Map<List<NotificationDto>>(items);

            var result = new PagedResult<NotificationDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = page,
                Limit = limit
            };

            return Result<PagedResult<NotificationDto>>.Success(result);
        }

        public async Task<Result> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Error);
            var user = userResult.Value;

            var profileId = user.MusicianProfile.Id;
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
                return Result.Failure("Уведомление не найдено");

            if (notification.ProfileId != profileId)
                return Result.Failure("Нет доступа к этому уведомлению");

            await _notificationRepository.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> MarkAllAsReadAsync(Guid userId)
        {
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Error);
            var user = userResult.Value;

            var profileId = user.MusicianProfile.Id;
            await _notificationRepository.MarkAllAsReadAsync(profileId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return 0;
            var user = userResult.Value;

            var profileId = user.MusicianProfile.Id;
            return await _notificationRepository.GetUnreadCountAsync(profileId);
        }
    }
}
