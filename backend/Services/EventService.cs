using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Events;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services
{
    /// <summary>
    /// Сервис для работы с мероприятиями
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IRegionRepository _regionRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorage _fileStorage;
        private readonly INotificationService _notificationService;

        public EventService(
            IEventRepository eventRepository,
            IUserRepository userRepository,
            IProfileRepository profileRepository,
            IRegionRepository regionRepository,
            ICityRepository cityRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileStorage fileStorage,
            INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _regionRepository = regionRepository;
            _cityRepository = cityRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _notificationService = notificationService;
        }

        public async Task<Result<PagedResult<EventDto>>> GetEventsAsync(EventFilterRequest filter)
        {
            var (items, totalCount) = await _eventRepository.SearchAsync(
                query: filter.Query,
                regionId: filter.RegionId,
                cityId: filter.CityId,
                fromDate: filter.FromDate,
                toDate: filter.ToDate,
                status: filter.Status,
                creatorProfileId: filter.CreatorProfileId,
                page: filter.Page,
                limit: filter.Limit,
                sortBy: filter.SortBy,
                sortDesc: filter.SortDesc);

            var dtos = _mapper.Map<List<EventDto>>(items);

            // Для каждого мероприятия получаем количество участников
            foreach (var dto in dtos)
            {
                var eventId = dto.Id;
                dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(eventId);
            }

            var result = new PagedResult<EventDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = filter.Page,
                Limit = filter.Limit
            };

            return Result<PagedResult<EventDto>>.Success(result);
        }

        public async Task<Result<EventDto>> GetByIdAsync(Guid id, Guid? currentUserId = null)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
                return Result<EventDto>.Failure("Мероприятие не найдено");

            var dto = _mapper.Map<EventDto>(eventEntity);
            dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(id);

            if (currentUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(currentUserId.Value);
                if (user?.MusicianProfile != null)
                {
                    dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(id, user.MusicianProfile.Id);
                }
            }

            return Result<EventDto>.Success(dto);
        }

        public async Task<Result<EventDto>> CreateAsync(Guid userId, CreateEventRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<EventDto>.Failure("Профиль пользователя не найден");

            var region = await _regionRepository.GetByIdAsync(request.RegionId);
            if (region == null)
                return Result<EventDto>.Failure("Регион не найден");

            var city = await _cityRepository.GetByIdAsync(request.CityId);
            if (city == null)
                return Result<EventDto>.Failure("Город не найден");

            if (request.StartDateTime < DateTime.UtcNow)
                return Result<EventDto>.Failure("Дата начала не может быть в прошлом");

            if (request.EndDateTime.HasValue && request.EndDateTime.Value < request.StartDateTime)
                return Result<EventDto>.Failure("Дата окончания не может быть раньше даты начала");

            if (request.MaxParticipants < 0)
                return Result<EventDto>.Failure("Максимальное количество участников не может быть отрицательным");

            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                RegionId = request.RegionId,
                CityId = request.CityId,
                Address = request.Address,
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                MaxParticipants = request.MaxParticipants,
                CreatorProfileId = user.MusicianProfile.Id,
                Status = EventStatus.Scheduled
            };

            await _eventRepository.AddAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<EventDto>(eventEntity);
            dto.CurrentParticipants = 0;
            dto.IsRegistered = false;

            return Result<EventDto>.Success(dto);
        }

        public async Task<Result<EventDto>> UpdateAsync(Guid userId, Guid eventId, UpdateEventRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<EventDto>.Failure("Профиль пользователя не найден");

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return Result<EventDto>.Failure("Мероприятие не найдено");

            if (eventEntity.CreatorProfileId != user.MusicianProfile.Id)
                return Result<EventDto>.Failure("Только создатель может редактировать мероприятие");

            if (eventEntity.Status != EventStatus.Scheduled)
                return Result<EventDto>.Failure("Можно редактировать только запланированные мероприятия");

            // Обновление полей
            if (!string.IsNullOrWhiteSpace(request.Title))
                eventEntity.Title = request.Title;
            if (request.Description != null)
                eventEntity.Description = request.Description;
            if (request.RegionId.HasValue)
            {
                var region = await _regionRepository.GetByIdAsync(request.RegionId.Value);
                if (region == null)
                    return Result<EventDto>.Failure("Регион не найден");
                eventEntity.RegionId = request.RegionId.Value;
            }
            if (request.CityId.HasValue)
            {
                var city = await _cityRepository.GetByIdAsync(request.CityId.Value);
                if (city == null)
                    return Result<EventDto>.Failure("Город не найден");
                eventEntity.CityId = request.CityId.Value;
            }
            if (!string.IsNullOrWhiteSpace(request.Address))
                eventEntity.Address = request.Address;
            if (request.StartDateTime.HasValue)
            {
                if (request.StartDateTime.Value < DateTime.UtcNow)
                    return Result<EventDto>.Failure("Дата начала не может быть в прошлом");
                eventEntity.StartDateTime = request.StartDateTime.Value;
            }
            if (request.EndDateTime.HasValue)
            {
                if (request.EndDateTime.Value < eventEntity.StartDateTime)
                    return Result<EventDto>.Failure("Дата окончания не может быть раньше даты начала");
                eventEntity.EndDateTime = request.EndDateTime;
            }
            if (request.MaxParticipants.HasValue)
            {
                if (request.MaxParticipants < 0)
                    return Result<EventDto>.Failure("Максимальное количество участников не может быть отрицательным");
                eventEntity.MaxParticipants = request.MaxParticipants.Value;
            }

            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<EventDto>(eventEntity);
            dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(eventId);
            dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(eventId, user.MusicianProfile.Id);

            return Result<EventDto>.Success(dto);
        }

        public async Task<Result> CancelAsync(Guid userId, Guid eventId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result.Failure("Профиль пользователя не найден");

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return Result.Failure("Мероприятие не найдено");

            if (eventEntity.CreatorProfileId != user.MusicianProfile.Id)
                return Result.Failure("Только создатель может отменить мероприятие");

            if (eventEntity.Status != EventStatus.Scheduled)
                return Result.Failure("Можно отменить только запланированное мероприятие");

            eventEntity.Status = EventStatus.Cancelled;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RegisterAsync(Guid userId, Guid eventId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result.Failure("Профиль пользователя не найден");

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return Result.Failure("Мероприятие не найдено");

            if (eventEntity.Status != EventStatus.Scheduled)
                return Result.Failure("Нельзя записаться на отменённое или завершённое мероприятие");

            if (eventEntity.StartDateTime < DateTime.UtcNow)
                return Result.Failure("Мероприятие уже началось");

            if (await _eventRepository.IsUserRegisteredAsync(eventId, user.MusicianProfile.Id))
                return Result.Failure("Вы уже записаны на это мероприятие");

            var currentCount = await _eventRepository.GetRegistrationCountAsync(eventId);
            if (eventEntity.MaxParticipants > 0 && currentCount >= eventEntity.MaxParticipants)
                return Result.Failure("Достигнут лимит участников");

            var registration = new EventRegistration
            {
                EventId = eventId,
                ProfileId = user.MusicianProfile.Id
            };

            await _eventRepository.AddRegistrationAsync(registration);
            await _unitOfWork.SaveChangesAsync();

            // Создаём уведомление создателю мероприятия (если это не он сам)
            if (eventEntity.CreatorProfileId != user.MusicianProfile.Id)
            {
                var registeredProfile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
                await _notificationService.SendNotificationToProfileAsync(
                    eventEntity.CreatorProfileId,
                    NotificationType.EventRegistration,
                    new Dictionary<string, object>
                    {
                        ["registeredProfileName"] = registeredProfile?.FullName ?? "Пользователь",
                        ["eventId"] = eventId,
                        ["eventTitle"] = eventEntity.Title
                    });
            }

            return Result.Success();
        }

        public async Task<Result> UnregisterAsync(Guid userId, Guid eventId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result.Failure("Профиль пользователя не найден");

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return Result.Failure("Мероприятие не найдено");

            if (eventEntity.Status != EventStatus.Scheduled)
                return Result.Failure("Нельзя отменить запись на отменённое или завершённое мероприятие");

            if (!await _eventRepository.IsUserRegisteredAsync(eventId, user.MusicianProfile.Id))
                return Result.Failure("Вы не записаны на это мероприятие");

            await _eventRepository.RemoveRegistrationAsync(eventId, user.MusicianProfile.Id);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<PagedResult<EventDto>>> GetMyCreatedEventsAsync(Guid userId, int page, int limit)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<PagedResult<EventDto>>.Failure("Профиль пользователя не найден");

            var (items, totalCount) = await _eventRepository.GetCreatedByProfileAsync(user.MusicianProfile.Id, page, limit);
            var dtos = _mapper.Map<List<EventDto>>(items);

            foreach (var dto in dtos)
            {
                dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(dto.Id);
                dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(dto.Id, user.MusicianProfile.Id);
            }

            var result = new PagedResult<EventDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = page,
                Limit = limit
            };

            return Result<PagedResult<EventDto>>.Success(result);
        }

        public async Task<Result<PagedResult<EventDto>>> GetMyRegisteredEventsAsync(Guid userId, int page, int limit)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<PagedResult<EventDto>>.Failure("Профиль пользователя не найден");

            var (items, totalCount) = await _eventRepository.GetRegisteredByProfileAsync(user.MusicianProfile.Id, page, limit);
            var dtos = _mapper.Map<List<EventDto>>(items);

            foreach (var dto in dtos)
            {
                dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(dto.Id);
                dto.IsRegistered = true;
            }

            var result = new PagedResult<EventDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = page,
                Limit = limit
            };

            return Result<PagedResult<EventDto>>.Success(result);
        }

        public async Task<Result<string>> UploadImageAsync(Guid userId, Guid eventId, Stream fileStream, string fileName, string contentType)
        {
            if (!contentType.StartsWith("image/"))
                return Result<string>.Failure("Разрешены только изображения");

            if (fileStream.Length > 5 * 1024 * 1024) // 5 MB
                return Result<string>.Failure("Файл слишком большой (макс. 5 МБ)");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<string>.Failure("Профиль пользователя не найден");

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return Result<string>.Failure("Мероприятие не найдено");

            if (eventEntity.CreatorProfileId != user.MusicianProfile.Id)
                return Result<string>.Failure("Только создатель может менять изображение");

            if (eventEntity.ImageUrl != null)
            {
                await _fileStorage.DeleteFileAsync(eventEntity.ImageUrl);
            }

            var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);
            eventEntity.ImageUrl = fileUrl;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(fileUrl);
        }
    }
}
