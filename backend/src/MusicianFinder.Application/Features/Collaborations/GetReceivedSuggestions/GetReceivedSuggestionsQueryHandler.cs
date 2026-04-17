using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Collaborations.DTOs;

namespace MusicianFinder.Application.Features.Collaborations.GetReceivedSuggestions
{
    /// <summary>
    /// Обработчик запроса <see cref="GetReceivedSuggestionsQuery"/>.
    /// </summary>
    public class GetReceivedSuggestionsQueryHandler : IRequestHandler<GetReceivedSuggestionsQuery, PagedResult<SuggestionDto>>
    {
        private readonly ICollaborationSuggestionRepository _suggestionRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetReceivedSuggestionsQueryHandler"/>.
        /// </summary>
        /// <param name="suggestionRepository">Репозиторий предложений.</param>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetReceivedSuggestionsQueryHandler(
            ICollaborationSuggestionRepository suggestionRepository,
            IProfileRepository profileRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _suggestionRepository = suggestionRepository;
            _profileRepository = profileRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> Handle(GetReceivedSuggestionsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль не найден.");

            var items = await _suggestionRepository.GetReceivedAsync(profile.Id, request.Page, request.Limit, request.SortBy, request.SortDesc);
            var dtos = _mapper.Map<List<SuggestionDto>>(items);

            return new PagedResult<SuggestionDto>
            {
                Items = dtos,
                Total = dtos.Count,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}