using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Events;
using backend.Models.DTOs.Uploads;
using backend.Models.Enums;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using backend.Services.Utils;

namespace backend.Services
{
    /// <summary>
    /// Сервис для работы с мероприятиями
    /// </summary>
    public class EventService(
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorage fileStorage,
        INotificationService notificationService,
        IValidator<CreateEventRequest> createValidator,
        IValidator<UpdateEventRequest> updateValidator,
        IEntityExistenceService entityExistenceService,
        IEventValidationService eventValidationService,
        IValidator<EventFilterRequest> filterValidator) : IEventService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IValidator<CreateEventRequest> _createValidator = createValidator;
        private readonly IValidator<UpdateEventRequest> _updateValidator = updateValidator;
        private readonly IEntityExistenceService _existenceService = entityExistenceService;
        private readonly IEventValidationService _eventValidator = eventValidationService;
        private readonly IValidator<EventFilterRequest> _filterValidator = filterValidator;

        public async Task<Result<PagedResult<EventDto>>> GetEventsAsync(EventFilterRequest filter, Guid? currentUserId = null)
        {
            var validationResult = await _filterValidator.ValidateAsync(filter);
            if (!validationResult.IsValid)
                return Result<PagedResult<EventDto>>.Failure(validationResult.ToErrorString());

            var (items, totalCount) = await _eventRepository.GetEventDtosAsync(filter, currentUserId);

            var result = new PagedResult<EventDto>
            {
                Items = items,
                Total = totalCount,
                Page = filter.Page,
                Limit = filter.Limit
            };

            return Result<PagedResult<EventDto>>.Success(result);
        }

        public async Task<Result<EventDto>> GetByIdAsync(Guid id, Guid? currentUserId = null)
        {
            var eventResult = await _existenceService.GetEventAsync(id);
            if (!eventResult.IsSuccess)
                return Result<EventDto>.Failure(eventResult.Error);
            var eventEntity = eventResult.Value;


            var dto = _mapper.Map<EventDto>(eventEntity);
            dto.CurrentParticipants = await _eventRepository.GetRegistrationCountAsync(id);

            if (currentUserId.HasValue)
            {
                var userResult = await _existenceService.GetUserWithProfileAsync(currentUserId ?? Guid.NewGuid());
                if (!userResult.IsSuccess)
                    return Result<EventDto>.Failure(userResult.Error);
                var user = userResult.Value;
                dto.IsRegistered = await _eventRepository.IsUserRegisteredAsync(id, user.MusicianProfile.Id);
            }

            return Result<EventDto>.Success(dto);
        }

        public async Task<Result<EventDto>> CreateAsync(Guid userId, CreateEventRequest request)
        {
            // Валидация DTO
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Result<EventDto>.Failure(validationResult.ToErrorString());

            // Получаем пользователя с профилем (сразу с проверкой)
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result<EventDto>.Failure(userResult.Error);
            var user = userResult.Value;

            // Проверяем регион и город, но объекты нам не нужны (можно использовать Validate или Get)
            var regionCheck = await _existenceService.ValidateRegionAsync(request.RegionId);
            if (!regionCheck.IsSuccess)
                return Result<EventDto>.Failure(regionCheck.Error);

            var cityCheck = await _existenceService.ValidateCityAsync(request.CityId);
            if (!cityCheck.IsSuccess)
                return Result<EventDto>.Failure(cityCheck.Error);

            // Бизнес-валидация дат
            var dateValidation = _eventValidator.ValidateEventDates(request.StartDateTime, request.EndDateTime);
            if (!dateValidation.IsSuccess)
                return Result<EventDto>.Failure(dateValidation.Error);

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
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result<EventDto>.Failure(validationResult.ToErrorString());
            }

            var userValidation = await _existenceService.ValidateUserWithProfileAsync(userId);
            if (!userValidation.IsSuccess)
                return Result<EventDto>.Failure(userValidation.Error);

            var eventValidation = await _existenceService.ValidateEventAsync(eventId);
            if (!eventValidation.IsSuccess)
                return Result<EventDto>.Failure(eventValidation.Error);

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            var user = await _userRepository.GetByIdAsync(userId);

            var ownershipCheck = _eventValidator.ValidateEventOwnership(eventEntity, user.MusicianProfile.Id);
            if (!ownershipCheck.IsSuccess)
                return Result<EventDto>.Failure(ownershipCheck.Error);

            var scheduledCheck = _eventValidator.ValidateEventIsScheduled(eventEntity);
            if (!scheduledCheck.IsSuccess)
                return Result<EventDto>.Failure(scheduledCheck.Error);


            if (!string.IsNullOrWhiteSpace(request.Title))
                eventEntity.Title = request.Title;
            if (request.Description != null)
                eventEntity.Description = request.Description;
            if (request.RegionId.HasValue)
            {
                var regionValidation = await _existenceService.ValidateRegionAsync(request.RegionId.Value);
                if (!regionValidation.IsSuccess)
                    return Result<EventDto>.Failure(regionValidation.Error);
                eventEntity.RegionId = request.RegionId.Value;
            }
            if (request.CityId.HasValue)
            {
                var cityValidation = await _existenceService.ValidateCityAsync(request.CityId.Value);
                if (!cityValidation.IsSuccess)
                    return Result<EventDto>.Failure(cityValidation.Error);
                eventEntity.CityId = request.CityId.Value;
            }
            if (!string.IsNullOrWhiteSpace(request.Address))
                eventEntity.Address = request.Address;
            if (request.StartDateTime.HasValue)
            {
                var dateCheck = _eventValidator.ValidateEventDates(request.StartDateTime.Value, request.EndDateTime ?? eventEntity.EndDateTime);
                if (!dateCheck.IsSuccess)
                    return Result<EventDto>.Failure(dateCheck.Error);
                eventEntity.StartDateTime = request.StartDateTime.Value;
            }
            if (request.EndDateTime.HasValue)
            {
                var dateCheck = _eventValidator.ValidateEventDates(request.StartDateTime ?? eventEntity.StartDateTime, request.EndDateTime);
                if (!dateCheck.IsSuccess)
                    return Result<EventDto>.Failure(dateCheck.Error);
                eventEntity.EndDateTime = request.EndDateTime;
            }
            if (request.MaxParticipants.HasValue)
            {
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
            var userValidation = await _existenceService.ValidateUserWithProfileAsync(userId);
            if (!userValidation.IsSuccess)
                return Result.Failure(userValidation.Error);

            var eventValidation = await _existenceService.ValidateEventAsync(eventId);
            if (!eventValidation.IsSuccess)
                return Result.Failure(eventValidation.Error);

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            var user = await _userRepository.GetByIdAsync(userId);

            var ownershipCheck = _eventValidator.ValidateEventOwnership(eventEntity, user.MusicianProfile.Id);
            if (!ownershipCheck.IsSuccess)
                return ownershipCheck;

            var scheduledCheck = _eventValidator.ValidateEventIsScheduled(eventEntity);
            if (!scheduledCheck.IsSuccess)
                return scheduledCheck;

            eventEntity.Status = EventStatus.Cancelled;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RegisterAsync(Guid userId, Guid eventId)
        {
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Error);
            var user = userResult.Value;

            var eventResult = await _existenceService.GetEventAsync(eventId);
            if (!eventResult.IsSuccess)
                return Result.Failure(eventResult.Error);
            var eventEntity = eventResult.Value;

            var scheduledCheck = _eventValidator.ValidateEventIsScheduled(eventEntity);
            if (!scheduledCheck.IsSuccess) return scheduledCheck;

            var notStartedCheck = _eventValidator.ValidateEventNotStarted(eventEntity);
            if (!notStartedCheck.IsSuccess) return notStartedCheck;

            if (await _eventRepository.IsUserRegisteredAsync(eventId, user.MusicianProfile.Id))
                return Result.Failure("Вы уже записаны на это мероприятие");

            var currentCount = await _eventRepository.GetRegistrationCountAsync(eventId);
            var capacityCheck = _eventValidator.ValidateEventCapacity(eventEntity, currentCount);
            if (!capacityCheck.IsSuccess) return capacityCheck;

            var registration = new EventRegistration { EventId = eventId, ProfileId = user.MusicianProfile.Id };
            await _eventRepository.AddRegistrationAsync(registration);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
            {
                return Result.Failure("Вы уже зарегистрированы на это мероприятие");
            }

            await _notificationService.SendNotificationToProfileAsync(
                user.MusicianProfile.Id,
                NotificationType.EventRegistration,
                new Dictionary<string, object> { ["eventId"] = eventId, ["eventTitle"] = eventEntity.Title });

            return Result.Success();
        }

        public async Task<Result> UnregisterAsync(Guid userId, Guid eventId)
        {
            var userValidation = await _existenceService.ValidateUserWithProfileAsync(userId);
            if (!userValidation.IsSuccess)
                return Result.Failure(userValidation.Error);

            var eventValidation = await _existenceService.ValidateEventAsync(eventId);
            if (!eventValidation.IsSuccess)
                return Result.Failure(eventValidation.Error);

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            var user = await _userRepository.GetByIdAsync(userId);

            var scheduledCheck = _eventValidator.ValidateEventIsScheduled(eventEntity);
            if (!scheduledCheck.IsSuccess)
                return scheduledCheck;

            if (!await _eventRepository.IsUserRegisteredAsync(eventId, user.MusicianProfile.Id))
                return Result.Failure("Вы не записаны на это мероприятие");

            await _eventRepository.RemoveRegistrationAsync(eventId, user.MusicianProfile.Id);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<PagedResult<EventDto>>> GetMyCreatedEventsAsync(Guid userId, int page, int limit)
        {
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result<PagedResult<EventDto>>.Failure(userResult.Error);
            var user = userResult.Value;

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
            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result<PagedResult<EventDto>>.Failure(userResult.Error);
            var user = userResult.Value;

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

            if (fileStream.Length > 5 * 1024 * 1024)
                return Result<string>.Failure("Файл слишком большой (макс. 5 МБ)");

            var userValidation = await _existenceService.ValidateUserWithProfileAsync(userId);
            if (!userValidation.IsSuccess)
                return Result<string>.Failure(userValidation.Error);

            var eventValidation = await _existenceService.ValidateEventAsync(eventId);
            if (!eventValidation.IsSuccess)
                return Result<string>.Failure(eventValidation.Error);

            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            var user = await _userRepository.GetByIdAsync(userId);

            var ownershipCheck = _eventValidator.ValidateEventOwnership(eventEntity, user.MusicianProfile.Id);
            if (!ownershipCheck.IsSuccess)
                return Result<string>.Failure(ownershipCheck.Error);

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

        /// <summary>
        /// Определяет, является ли исключение нарушением уникальности (дубликат ключа).
        /// Для PostgreSQL анализируется PostgresException с SqlState = "23505".
        /// </summary>
        private static bool IsDuplicateKeyException(DbUpdateException ex)
        {
            var inner = ex.InnerException;
            while (inner != null)
            {
                if (inner is PostgresException postgresEx && postgresEx.SqlState == "23505")
                    return true;
                inner = inner.InnerException;
            }
            return false;
        }
    }
}
