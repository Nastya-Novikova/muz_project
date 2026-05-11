using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Реализация <see cref="ICurrentProfileProvider"/>.
    /// Получает профиль текущего пользователя, используя <see cref="ICurrentUserService"/>
    /// и <see cref="IMusicianProfileRepository"/>.
    /// </summary>
    public class CurrentProfileProvider : ICurrentProfileProvider
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMusicianProfileRepository _profileRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CurrentProfileProvider"/>.
        /// </summary>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="profileRepository">Репозиторий профилей музыкантов.</param>
        public CurrentProfileProvider(
            ICurrentUserService currentUserService,
            IMusicianProfileRepository profileRepository)
        {
            _currentUserService = currentUserService;
            _profileRepository = profileRepository;
        }

        /// <inheritdoc />
        public async Task<MusicianProfile> GetCurrentProfileAsync(CancellationToken ct = default)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new ForbiddenException("Пользователь не аутентифицирован.");

            var userId = _currentUserService.UserId;
            var profile = await _profileRepository.GetByUserIdAsync(userId, ct)
                          ?? throw new NotFoundException("Профиль не найден.");

            return profile;
        }
    }
}