using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.ValueObjects;
using FluentValidation.Results;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Guid>
    {
        private readonly ICurrentProfileProvider _profileProvider;
        private readonly IReferenceDataValidationService _referenceDataValidation;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileProvider">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public UpdateProfileCommandHandler(ICurrentProfileProvider profileProvider, IReferenceDataValidationService referenceDataValidation)
        {
            _profileProvider = profileProvider;
            _referenceDataValidation = referenceDataValidation;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileProvider.GetCurrentProfileAsync(cancellationToken);

            await ValidateReferenceDataAsync(request, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.FullName) || request.Age.HasValue ||
                !string.IsNullOrWhiteSpace(request.Description) || request.CityId.HasValue)
            {
                profile.UpdateCoreInfo(
                    request.FullName != null ? new ProfileName(request.FullName) : profile.FullName,
                    request.Age ?? profile.Age,
                    request.Description ?? profile.Description,
                    request.CityId ?? profile.CityId);
            }
            if (!string.IsNullOrWhiteSpace(request.Phone) || !string.IsNullOrWhiteSpace(request.Telegram))
            {
                profile.UpdateContacts(
                    request.Phone != null ? new PhoneNumber(request.Phone) : profile.Phone,
                    request.Telegram != null ? new TelegramHandle(request.Telegram) : profile.Telegram);
            }
            if (request.GenreIds != null)
                profile.SetGenres(request.GenreIds.Select(id => new GenreId(id)));
            if (request.SpecialtyIds != null)
                profile.SetSpecialties(request.SpecialtyIds.Select(id => new SpecialtyId(id)));
            if (request.CollaborationGoalIds != null)
                profile.SetCollaborationGoals(request.CollaborationGoalIds.Select(id => new CollaborationGoalId(id)));
            if (request.DesiredGenreIds != null)
                profile.SetDesiredGenres(request.DesiredGenreIds.Select(id => new GenreId(id)));
            if (request.DesiredSpecialtyIds != null)
                profile.SetDesiredSpecialties(request.DesiredSpecialtyIds.Select(id => new SpecialtyId(id)));
            if (request.Experience.HasValue)
                profile.SetExperience(request.Experience.Value);
            if (request.LookingFor.HasValue)
                profile.SetLookingFor(request.LookingFor.Value);
            if (request.ProfileType.HasValue)
                profile.SetProfileType(request.ProfileType.Value);

            return profile.Id;
        }

        /// <summary>
        /// Проверяет существование справочных идентификаторов, если они заданы в команде.
        /// </summary>
        /// <param name="command">Команда обновления профиля.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <exception cref="ValidationException">Если какой-либо идентификатор не найден.</exception>
        private async Task ValidateReferenceDataAsync(UpdateProfileCommand command, CancellationToken ct)
        {
            var errors = new List<string>();

            if (command.CityId.HasValue)
                if (!await _referenceDataValidation.CityExistsAsync(command.CityId.Value, ct))
                    errors.Add($"Город с ID {command.CityId.Value} не существует.");

            if (command.GenreIds != null)
                foreach (var genreId in command.GenreIds)
                    if (!await _referenceDataValidation.GenreExistsAsync(genreId, ct))
                        errors.Add($"Жанр с ID {genreId} не существует.");

            if (command.SpecialtyIds != null)
                foreach (var specialtyId in command.SpecialtyIds)
                    if (!await _referenceDataValidation.SpecialtyExistsAsync(specialtyId, ct))
                        errors.Add($"Специальность с ID {specialtyId} не существует.");

            if (command.CollaborationGoalIds != null)
                foreach (var goalId in command.CollaborationGoalIds)
                    if (!await _referenceDataValidation.CollaborationGoalExistsAsync(goalId, ct))
                        errors.Add($"Цель сотрудничества с ID {goalId} не существует.");

            if (command.DesiredGenreIds != null)
                foreach (var desiredGenreId in command.DesiredGenreIds)
                    if (!await _referenceDataValidation.GenreExistsAsync(desiredGenreId, ct))
                        errors.Add($"Искомый жанр с ID {desiredGenreId} не существует.");

            if (command.DesiredSpecialtyIds != null)
                foreach (var desiredSpecialtyId in command.DesiredSpecialtyIds)
                    if (!await _referenceDataValidation.SpecialtyExistsAsync(desiredSpecialtyId, ct))
                        errors.Add($"Искомая специальность с ID {desiredSpecialtyId} не существует.");

            if (errors.Count > 0)
                throw new ValidationException(errors.Select(e => new ValidationFailure("ReferenceData", e)));
        }
    }
}