using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="SearchProfilesQuery"/>.
    /// </summary>
    public class SearchProfilesQueryHandler : IRequestHandler<SearchProfilesQuery, PagedResult<ProfileDto>>
    {
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="profileReadRepository">Репозиторий для чтения профилей.</param>
        public SearchProfilesQueryHandler(IProfileReadRepository profileReadRepository)
        {
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> Handle(SearchProfilesQuery request, CancellationToken cancellationToken)
        {
            return await _profileReadRepository.SearchAsync(request, cancellationToken);
        }
    }
}