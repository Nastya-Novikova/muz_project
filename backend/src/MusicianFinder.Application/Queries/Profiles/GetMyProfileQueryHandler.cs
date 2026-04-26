using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetMyProfileQuery"/>.
    /// Возвращает профиль текущего пользователя.
    /// </summary>
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileDto>
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
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
            var dto = await _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            dto.IsMyProfile = true;
            return dto;
        }
    }
}