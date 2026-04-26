using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Favorites
{
    /// <summary>
    /// Обработчик запроса <see cref="GetFavoritesQuery"/>.
    /// </summary>
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PagedResult<ProfileDto>>
    {
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="favoriteReadRepository">Репозиторий для чтения избранного.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetFavoritesQueryHandler(IFavoriteReadRepository favoriteReadRepository, ICurrentUserService currentUser, IProfileReadRepository profileReadRepository, ICollaborationSuggestionReadRepository collaborationSuggestionReadRepository)
        {
            _favoriteReadRepository = favoriteReadRepository;
            _currentUser = currentUser;
            _profileReadRepository = profileReadRepository;
            _suggestionReadRepository = collaborationSuggestionReadRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileReadRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                      ?? throw new NotFoundException("Профиль не найден.");
            var result = await _favoriteReadRepository.GetFavoritesAsync(profile.Id, request.Page, request.Limit, cancellationToken);

            if (result.Items.Any())
            {
                var profileIds = result.Items.Select(p => p.Id).ToList();

                var collaboratedIds = await _suggestionReadRepository.GetSentSuggestionToProfileIdsAsync(profile.Id, profileIds, cancellationToken);

                foreach (var dto in result.Items)
                {
                    dto.IsMyProfile = dto.Id == profile.Id;
                    dto.IsFavorite = true;
                    dto.IsCollaborated = collaboratedIds.Contains(dto.Id);
                }
            }

            return result;
        }
    }
}