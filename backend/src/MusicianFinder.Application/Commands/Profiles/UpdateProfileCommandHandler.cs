using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly IMusicianProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public UpdateProfileCommandHandler(
            IMusicianProfileRepository profileRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

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

            return Unit.Value;
        }
    }
}