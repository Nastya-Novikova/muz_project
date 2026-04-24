using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetGenresQuery"/>.
    /// </summary>
    public class GetGenresQueryHandler : IRequestHandler<GetGenresQuery, List<LookupItemDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetGenresQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        /// <param name="cache">Сервис кеша.</param>
        public GetGenresQueryHandler(IReadDbContext dbContext, IMapper mapper, ICacheService cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = "reference:genres";
            var cached = await _cache.GetAsync<List<LookupItemDto>>(cacheKey);
            if (cached != null) return cached;

            var query = _dbContext.Genres.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(g => g.Name.Contains(request.Query) || g.LocalizedName.Contains(request.Query));

            query = ApplySorting(query, request.SortBy, request.SortDesc);
            var genres = await query.ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<LookupItemDto>>(genres);
            await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromHours(1));
            return dtos;
        }

        private static IQueryable<Genre> ApplySorting(IQueryable<Genre> query, string? sortBy, bool sortDesc)
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