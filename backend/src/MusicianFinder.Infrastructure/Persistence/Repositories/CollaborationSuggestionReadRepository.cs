using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для предложений о сотрудничестве.
    /// </summary>
    public class CollaborationSuggestionReadRepository : ICollaborationSuggestionReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CollaborationSuggestionReadRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public CollaborationSuggestionReadRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> GetReceivedAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            var query = _dbContext.CollaborationSuggestions.AsNoTracking()
                .Where(cs => cs.ToProfileId == profileId)
                .OrderByDescending(cs => cs.CreatedAt);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * limit).Take(limit)
                .ProjectTo<SuggestionDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

            return new PagedResult<SuggestionDto> { Items = items, Total = total, Page = page, Limit = limit };
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> GetSentAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            var query = _dbContext.CollaborationSuggestions.AsNoTracking()
                .Where(cs => cs.FromProfileId == profileId)
                .OrderByDescending(cs => cs.CreatedAt);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * limit).Take(limit)
                .ProjectTo<SuggestionDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

            return new PagedResult<SuggestionDto> { Items = items, Total = total, Page = page, Limit = limit };
        }
    }
}