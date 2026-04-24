using MediatR;
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

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="suggestionReadRepository">Репозиторий для чтения предложений.</param>
        /// <param name="currentUser">Сервис текущего пользователя.</param>
        public GetReceivedSuggestionsQueryHandler(
            ICollaborationSuggestionReadRepository suggestionReadRepository,
            ICurrentUserService currentUser)
        {
            _suggestionReadRepository = suggestionReadRepository;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> Handle(GetReceivedSuggestionsQuery request, CancellationToken cancellationToken)
        {
            return await _suggestionReadRepository.GetReceivedAsync(_currentUser.UserId, request.Page, request.Limit, cancellationToken);
        }
    }
}