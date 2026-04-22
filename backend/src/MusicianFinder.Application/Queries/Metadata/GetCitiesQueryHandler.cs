using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetCitiesQuery"/>.
    /// </summary>
    public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, List<LookupItemDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetCitiesQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public GetCitiesQueryHandler(IReadDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Cities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(c => c.Name.Contains(request.Query) || c.LocalizedName.Contains(request.Query));

            query = ApplySorting(query, request.SortBy, request.SortDesc);

            var cities = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<LookupItemDto>>(cities);
        }

        private static IQueryable<City> ApplySorting(IQueryable<City> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => sortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "localizedname" => sortDesc ? query.OrderByDescending(c => c.LocalizedName) : query.OrderBy(c => c.LocalizedName),
                _ => query.OrderBy(c => c.Id)
            };
        }
    }
}