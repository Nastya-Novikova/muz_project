using MediatR;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetProfileByIdQuery"/>.
    /// </summary>
    public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto>
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileReadRepository">Репозиторий для чтения профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public GetProfileByIdQueryHandler(
            IProfileReadRepository profileReadRepository,
            ICurrentUserService currentUserService)
        {
            _profileReadRepository = profileReadRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _profileReadRepository.GetByIdAsync(request.ProfileId, cancellationToken)
                ?? throw new Application.Core.Exceptions.NotFoundException("Профиль не найден.");

            if (_currentUserService.IsAuthenticated)
            {
                // Можно добавить дополнительные поля IsMyProfile, IsFavorite и т.д.
            }

            return dto;
        }
    }
}