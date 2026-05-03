using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;
using FluentValidation.Results;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="CreateProfileCommand"/>.
    /// </summary>
    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Guid>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IReferenceDataValidationService _referenceDataValidation;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public CreateProfileCommandHandler(
            IMusicianProfileRepository profileRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IReferenceDataValidationService referenceDataValidation)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _referenceDataValidation = referenceDataValidation;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");

            if (user.ProfileCreated)
                throw new ConflictException("Профиль уже создан.");

            await ValidateReferenceDataAsync(request, cancellationToken);

            var profile = MusicianProfile.Create(user.Id, new ProfileName(request.FullName), request.CityId, user.Email, request.ProfileType);

            profile.UpdateCoreInfo(new ProfileName(request.FullName), request.Age, request.Description, request.CityId);
            profile.UpdateContacts(
                request.Phone != null ? new PhoneNumber(request.Phone) : null,
                request.Telegram != null ? new TelegramHandle(request.Telegram) : null);
            
            var genreIds = request.GenreIds ?? new List<int>();
            var specialtyIds = request.SpecialtyIds ?? new List<int>();
            var collaborationGoalIds = request.CollaborationGoalIds ?? new List<int>();
            var desiredGenreIds = request.DesiredGenreIds ?? new List<int>();
            var desiredSpecialtyIds = request.DesiredSpecialtyIds ?? new List<int>();

            profile.SetGenres(genreIds.Select(id => new GenreId(id)));
            profile.SetSpecialties(specialtyIds.Select(id => new SpecialtyId(id)));
            profile.SetCollaborationGoals(collaborationGoalIds.Select(id => new CollaborationGoalId(id)));
            profile.SetDesiredGenres(desiredGenreIds.Select(id => new GenreId(id)));
            profile.SetDesiredSpecialties(desiredSpecialtyIds.Select(id => new SpecialtyId(id)));

            profile.SetExperience(request.Experience ?? 0);
            profile.SetLookingFor(request.LookingFor ?? Domain.Enums.LookingFor.NotLooking);

            _profileRepository.Add(profile);
            user.MarkProfileAsCreated();

            return profile.Id;
        }

        /// <summary>
        /// Проверяет существование всех переданных идентификаторов справочников.
        /// </summary>
        /// <param name="command">Команда создания профиля.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <exception cref="ValidationException">Если какой-либо идентификатор не найден.</exception>
        private async Task ValidateReferenceDataAsync(CreateProfileCommand command, CancellationToken ct)
        {
            var errors = new List<string>();

            if (!await _referenceDataValidation.CityExistsAsync(command.CityId, ct))
                errors.Add($"Город с ID {command.CityId} не существует.");

            foreach (var genreId in command.GenreIds)
                if (!await _referenceDataValidation.GenreExistsAsync(genreId, ct))
                    errors.Add($"Жанр с ID {genreId} не существует.");

            foreach (var specialtyId in command.SpecialtyIds)
                if (!await _referenceDataValidation.SpecialtyExistsAsync(specialtyId, ct))
                    errors.Add($"Специальность с ID {specialtyId} не существует.");

            foreach (var goalId in command.CollaborationGoalIds)
                if (!await _referenceDataValidation.CollaborationGoalExistsAsync(goalId, ct))
                    errors.Add($"Цель сотрудничества с ID {goalId} не существует.");

            foreach (var desiredGenreId in command.DesiredGenreIds)
                if (!await _referenceDataValidation.GenreExistsAsync(desiredGenreId, ct))
                    errors.Add($"Искомый жанр с ID {desiredGenreId} не существует.");

            foreach (var desiredSpecialtyId in command.DesiredSpecialtyIds)
                if (!await _referenceDataValidation.SpecialtyExistsAsync(desiredSpecialtyId, ct))
                    errors.Add($"Искомая специальность с ID {desiredSpecialtyId} не существует.");

            if (errors.Count > 0)
                throw new ValidationException(errors.Select(e => new ValidationFailure("ReferenceData", e)));
        }
    }
}