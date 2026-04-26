using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="GetProfileByIdQuery"/>.
    /// Возвращает детальную информацию о профиле, включая флаги для авторизованного пользователя.
    /// </summary>
    public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, ProfileDto>
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        public GetProfileByIdQueryHandler(
            IProfileReadRepository profileReadRepository,
            ICurrentUserService currentUserService,
            IFavoriteReadRepository favoriteReadRepository,
            ICollaborationSuggestionReadRepository suggestionReadRepository)
        {
            _profileReadRepository = profileReadRepository;
            _currentUserService = currentUserService;
            _favoriteReadRepository = favoriteReadRepository;
            _suggestionReadRepository = suggestionReadRepository;
        }

        /// <inheritdoc />
        public async Task<ProfileDto> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _profileReadRepository.GetByIdAsync(request.ProfileId, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            if (_currentUserService.IsAuthenticated)
            {
                var myProfile = await _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
                if (myProfile != null)
                {
                    dto.IsMyProfile = myProfile.Id == dto.Id;

                    var favSet = await _favoriteReadRepository.GetFavoritedProfileIdsAsync(myProfile.Id, new[] { dto.Id }, cancellationToken);
                    dto.IsFavorite = favSet.Contains(dto.Id);

                    var sentSet = await _suggestionReadRepository.GetSentSuggestionToProfileIdsAsync(myProfile.Id, new[] { dto.Id }, cancellationToken);
                    dto.IsCollaborated = sentSet.Contains(dto.Id);
                }
            }

            return dto;
        }
    }
}