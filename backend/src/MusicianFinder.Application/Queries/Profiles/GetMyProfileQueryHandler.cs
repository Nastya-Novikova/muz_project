using MediatR;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyProfileQuery"/>.
    /// </summary>
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileDto>
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileReadRepository">Репозиторий для чтения профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public GetMyProfileQueryHandler(
            IProfileReadRepository profileReadRepository,
            ICurrentUserService currentUserService)
        {
            _profileReadRepository = profileReadRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var dto = await _profileReadRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            dto.IsMyProfile = true;

            return dto;
        }
    }
}