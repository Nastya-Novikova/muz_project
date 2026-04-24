using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Suggestions
{
    /// <summary>
    /// Обработчик запроса <see cref="GetSentSuggestionsQuery"/>.
    /// </summary>
    public class GetSentSuggestionsQueryHandler : IRequestHandler<GetSentSuggestionsQuery, PagedResult<SuggestionDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetSentSuggestionsQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetSentSuggestionsQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> Handle(GetSentSuggestionsQuery request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль не найден.");

            var query = _dbContext.CollaborationSuggestions
                .AsNoTracking()
                .Where(s => s.FromProfileId == profile.Id);

            query = ApplySorting(query, request.SortBy, request.SortDesc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ProjectTo<SuggestionDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PagedResult<SuggestionDto>
            {
                Items = items,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }

        private static IQueryable<CollaborationSuggestion> ApplySorting(IQueryable<CollaborationSuggestion> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "status" => sortDesc ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
                "createdat" => sortDesc ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                _ => query.OrderByDescending(s => s.CreatedAt)
            };
        }
    }
}