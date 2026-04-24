using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetRegionsQuery"/>.
    /// </summary>
    public class GetRegionsQueryHandler : IRequestHandler<GetRegionsQuery, List<LookupItemDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetRegionsQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        /// <param name="cache">Сервис кеша.</param>
        public GetRegionsQueryHandler(IReadDbContext dbContext, IMapper mapper, ICacheService cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = "reference:regions";
            var cached = await _cache.GetAsync<List<LookupItemDto>>(cacheKey);
            if (cached != null) return cached;

            var query = _dbContext.Regions.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(r => r.Name.Contains(request.Query) || r.LocalizedName.Contains(request.Query));

            query = ApplySorting(query, request.SortBy, request.SortDesc);
            var regions = await query.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<LookupItemDto>>(regions);
            await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromHours(1));
            return dtos;
        }

        private static IQueryable<Region> ApplySorting(IQueryable<Region> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => sortDesc ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                "localizedname" => sortDesc ? query.OrderByDescending(r => r.LocalizedName) : query.OrderBy(r => r.LocalizedName),
                _ => query.OrderBy(r => r.Id)
            };
        }
    }
}