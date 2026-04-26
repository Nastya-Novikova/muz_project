using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="SearchProfilesQuery"/>.
    /// Выполняет поиск профилей с фильтрацией, скрывает собственный профиль и добавляет флаги.
    /// </summary>
    public class SearchProfilesQueryHandler : IRequestHandler<SearchProfilesQuery, PagedResult<ProfileDto>>
    {
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFavoriteReadRepository _favoriteReadRepository;
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        public SearchProfilesQueryHandler(
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
        public async Task<PagedResult<ProfileDto>> Handle(SearchProfilesQuery request, CancellationToken cancellationToken)
        {
            var result = await _profileReadRepository.SearchAsync(request, cancellationToken);

            if (_currentUserService.IsAuthenticated)
            {
                var myProfile = await _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);
                if (myProfile != null)
                {
                    result.Items = result.Items.Where(p => p.Id != myProfile.Id).ToList();
                    result.Total = result.Items.Count;

                    if (result.Items.Any())
                    {
                        var profileIds = result.Items.Select(p => p.Id).ToList();
                        var favSet = await _favoriteReadRepository.GetFavoritedProfileIdsAsync(myProfile.Id, profileIds, cancellationToken);
                        var sentSet = await _suggestionReadRepository.GetSentSuggestionToProfileIdsAsync(myProfile.Id, profileIds, cancellationToken);

                        foreach (var dto in result.Items)
                        {
                            dto.IsMyProfile = false;
                            dto.IsFavorite = favSet.Contains(dto.Id);
                            dto.IsCollaborated = sentSet.Contains(dto.Id);
                        }
                    }
                }
            }

            return result;
        }
    }
}