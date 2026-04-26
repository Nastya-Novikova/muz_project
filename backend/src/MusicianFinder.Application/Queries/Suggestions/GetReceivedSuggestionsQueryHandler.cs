using MediatR;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Application.Queries.Suggestions
{
    /// <summary>
    /// Обработчик запроса <see cref="GetReceivedSuggestionsQuery"/>.
    /// </summary>
    public class GetReceivedSuggestionsQueryHandler : IRequestHandler<GetReceivedSuggestionsQuery, PagedResult<SuggestionDto>>
    {
        private readonly ICollaborationSuggestionReadRepository _suggestionReadRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IProfileReadRepository _profileReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="suggestionReadRepository">Репозиторий для чтения предложений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetReceivedSuggestionsQueryHandler(
            ICollaborationSuggestionReadRepository suggestionReadRepository,
            ICurrentUserService currentUser, IProfileReadRepository profileReadRepository)
        {
            _suggestionReadRepository = suggestionReadRepository;
            _currentUser = currentUser;
            _profileReadRepository = profileReadRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> Handle(GetReceivedSuggestionsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileReadRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken)
                      ?? throw new NotFoundException("Профиль не найден.");
            return await _suggestionReadRepository.GetReceivedAsync(profile.Id, request.Page, request.Limit, cancellationToken);
        }
    }
}