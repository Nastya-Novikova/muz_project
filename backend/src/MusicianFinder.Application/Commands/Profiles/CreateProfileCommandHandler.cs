using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.ValueObjects;

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

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public CreateProfileCommandHandler(
            IMusicianProfileRepository profileRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Пользователь не найден.");

            if (user.ProfileCreated)
                throw new Application.Core.Exceptions.ConflictException("Профиль уже создан.");

            var profile = MusicianProfile.Create(user.Id, request.FullName, request.CityId);

            profile.UpdateCoreInfo(request.FullName, request.Age, request.Description, request.CityId);
            profile.UpdateContacts(
                request.Phone != null ? new PhoneNumber(request.Phone) : null,
                request.Telegram != null ? new TelegramHandle(request.Telegram) : null);
            profile.SetGenres(request.GenreIds);
            profile.SetSpecialties(request.SpecialtyIds);
            profile.SetCollaborationGoals(request.CollaborationGoalIds);
            profile.SetDesiredGenres(request.DesiredGenreIds);
            profile.SetDesiredSpecialties(request.DesiredSpecialtyIds);

            _profileRepository.Add(profile);
            user.MarkProfileAsCreated();

            return profile.Id;
        }
    }
}