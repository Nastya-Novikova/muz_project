using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;
using MusicianFinder.SharedKernel;
using FluentValidation.Results;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateEventCommand"/>.
    /// </summary>
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Guid>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IReferenceDataValidationService _referenceDataValidation;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        public UpdateEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider,
            IReferenceDataValidationService referenceDataValidation)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
            _referenceDataValidation = referenceDataValidation;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var @event = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken)
                ?? throw new NotFoundException("Мероприятие не найдено.");

            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            if (@event.CreatorProfileId != profile.Id)
                throw new ForbiddenException("Только создатель может редактировать мероприятие.");

            if (@event.Status != EventStatus.Scheduled)
                throw new DomainException("Редактировать можно только запланированное мероприятие.");

            await ValidateReferenceDataAsync(request, cancellationToken);

            var newTitle = request.Title != null ? new EventTitle(request.Title) : @event.Title;
            var newDescription = request.Description ?? @event.Description;
            var newRegionId = request.RegionId ?? @event.RegionId;
            var newCityId = request.CityId ?? @event.CityId;
            var newAddress = request.Address ?? @event.Address;
            var newStart = request.StartDateTime ?? @event.StartDateTime;
            var newEnd = request.EndDateTime ?? @event.EndDateTime;
            var newMaxParticipants = request.MaxParticipants ?? @event.MaxParticipants;

            if (newStart <= DateTime.UtcNow)
                throw new DomainException("Дата начала должна быть в будущем.");

            @event.Update(newTitle, newDescription, newRegionId, newCityId, newAddress,
                          newStart, newEnd, newMaxParticipants, profile.Id);

            return @event.Id;
        }

        /// <summary>
        /// Проверяет существование региона и города, если они заданы в команде.
        /// </summary>
        /// <param name="command">Команда обновления мероприятия.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <exception cref="ValidationException">Если регион или город не найдены.</exception>
        private async Task ValidateReferenceDataAsync(UpdateEventCommand command, CancellationToken ct)
        {
            var errors = new List<string>();

            if (command.RegionId.HasValue)
                if (!await _referenceDataValidation.RegionExistsAsync(command.RegionId.Value, ct))
                    errors.Add($"Регион с ID {command.RegionId.Value} не существует.");
            if (command.CityId.HasValue)
                if (!await _referenceDataValidation.CityExistsAsync(command.CityId.Value, ct))
                    errors.Add($"Город с ID {command.CityId.Value} не существует.");

            if (errors.Count > 0)
                throw new ValidationException(errors.Select(e => new ValidationFailure("ReferenceData", e)));
        }
    }
}