using MediatR;
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
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            profile.UpdateCoreInfo(request.FullName, request.Age, request.Description, request.CityId);
            profile.UpdateContacts(
                request.Phone != null ? new PhoneNumber(request.Phone) : null,
                request.Telegram != null ? new TelegramHandle(request.Telegram) : null);
            profile.SetGenres(request.GenreIds);
            profile.SetSpecialties(request.SpecialtyIds);
            profile.SetCollaborationGoals(request.CollaborationGoalIds);
            profile.SetDesiredGenres(request.DesiredGenreIds);
            profile.SetDesiredSpecialties(request.DesiredSpecialtyIds);

            return Unit.Value;
        }
    }
}