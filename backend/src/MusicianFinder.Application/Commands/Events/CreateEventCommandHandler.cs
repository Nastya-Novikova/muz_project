using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;
using FluentValidation.Results;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Обработчик команды <see cref="CreateEventCommand"/>.
    /// </summary>
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
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
        public CreateEventCommandHandler(
            IEventRepository eventRepository,
            ICurrentProfileProvider profileProvider,
            IReferenceDataValidationService referenceDataValidation)
        {
            _eventRepository = eventRepository;
            _profileProvider = profileProvider;
            _referenceDataValidation = referenceDataValidation;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            await ValidateReferenceDataAsync(request, cancellationToken);

            var newEvent = new Event(
                new EventTitle(request.Title),
                request.RegionId,
                request.CityId,
                request.Address,
                request.StartDateTime,
                profile.Id,
                request.Description,
                request.EndDateTime,
                request.MaxParticipants);

            _eventRepository.Add(newEvent);
            return newEvent.Id;
        }

        /// <summary>
        /// Проверяет существование региона и города, указанных в команде.
        /// </summary>
        /// <param name="command">Команда создания мероприятия.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <exception cref="ValidationException">Если регион или город не найдены.</exception>
        private async Task ValidateReferenceDataAsync(CreateEventCommand command, CancellationToken ct)
        {
            var errors = new List<string>();

            if (!await _referenceDataValidation.RegionExistsAsync(command.RegionId, ct))
                errors.Add($"Регион с ID {command.RegionId} не существует.");
            if (!await _referenceDataValidation.CityExistsAsync(command.CityId, ct))
                errors.Add($"Город с ID {command.CityId} не существует.");

            if (errors.Count > 0)
                throw new ValidationException(errors.Select(e => new ValidationFailure("ReferenceData", e)));
        }
    }
}