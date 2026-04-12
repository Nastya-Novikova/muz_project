using AutoMapper;
using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Notifications;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Models.Classes;

namespace backend.Services
{
    /// <summary>
    /// Сервис для работы с уведомлениями
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            IProfileRepository profileRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateCollaborationReceivedAsync(Guid recipientProfileId, Guid suggestionId, string fromProfileName)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ProfileId = recipientProfileId,
                Type = NotificationType.CollaborationReceived,
                Title = $"Пользователь {fromProfileName} отправил вам предложение о сотрудничестве",
                Message = "У вас новое предложение о сотрудничестве",
                EntityType = EntityType.CollaborationSuggestion,
                EntityId = suggestionId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateEventRegistrationAsync(Guid eventCreatorProfileId, Guid eventId, string eventTitle, Guid registeredProfileId)
        {
            var registeredProfile = await _profileRepository.GetByIdAsync(registeredProfileId);
            var profileName = registeredProfile?.FullName ?? "Пользователь";

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ProfileId = eventCreatorProfileId,
                Type = NotificationType.EventRegistration,
                Title = $"{profileName} записался на ваше мероприятие \"{eventTitle}\"",
                Message = "Новый участник зарегистрировался на ваше мероприятие",
                EntityType = EntityType.Event,
                EntityId = eventId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateEventReminderAsync(Guid profileId, Guid eventId, string eventTitle, int daysLeft)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ProfileId = profileId,
                Type = NotificationType.EventReminder,
                Title = $"Через {daysLeft} дн. состоится мероприятие \"{eventTitle}\"",
                Message = "Не забудьте о предстоящем мероприятии",
                EntityType = EntityType.Event,
                EntityId = eventId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<PagedResult<NotificationDto>>> GetUserNotificationsAsync(Guid userId, int page, int limit)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<PagedResult<NotificationDto>>.Failure("Профиль пользователя не найден");

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
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result.Failure("Профиль пользователя не найден");

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
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result.Failure("Профиль пользователя не найден");

            var profileId = user.MusicianProfile.Id;
            await _notificationRepository.MarkAllAsReadAsync(profileId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return 0;

            var profileId = user.MusicianProfile.Id;
            return await _notificationRepository.GetUnreadCountAsync(profileId);
        }
    }
}
