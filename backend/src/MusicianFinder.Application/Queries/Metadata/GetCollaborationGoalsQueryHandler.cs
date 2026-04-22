using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetCollaborationGoalsQuery"/>.
    /// </summary>
    public class GetCollaborationGoalsQueryHandler : IRequestHandler<GetCollaborationGoalsQuery, List<LookupItemDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetCollaborationGoalsQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public GetCollaborationGoalsQueryHandler(IReadDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetCollaborationGoalsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.CollaborationGoals.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(g => g.Name.Contains(request.Query) || g.LocalizedName.Contains(request.Query));

            query = ApplySorting(query, request.SortBy, request.SortDesc);

            var goals = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<LookupItemDto>>(goals);
        }

        private static IQueryable<CollaborationGoal> ApplySorting(IQueryable<CollaborationGoal> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => sortDesc ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
                "localizedname" => sortDesc ? query.OrderByDescending(g => g.LocalizedName) : query.OrderBy(g => g.LocalizedName),
                _ => query.OrderBy(g => g.Id)
            };
        }
    }
}