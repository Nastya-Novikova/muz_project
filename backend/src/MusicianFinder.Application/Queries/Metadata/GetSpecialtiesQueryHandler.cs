using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Обработчик запроса <see cref="GetSpecialtiesQuery"/>.
    /// </summary>
    public class GetSpecialtiesQueryHandler : IRequestHandler<GetSpecialtiesQuery, List<LookupItemDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetSpecialtiesQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public GetSpecialtiesQueryHandler(IReadDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetSpecialtiesQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Specialties.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(s => s.Name.Contains(request.Query) || s.LocalizedName.Contains(request.Query));

            query = ApplySorting(query, request.SortBy, request.SortDesc);

            var specialties = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<LookupItemDto>>(specialties);
        }

        private static IQueryable<MusicalSpecialty> ApplySorting(IQueryable<MusicalSpecialty> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => sortDesc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                "localizedname" => sortDesc ? query.OrderByDescending(s => s.LocalizedName) : query.OrderBy(s => s.LocalizedName),
                _ => query.OrderBy(s => s.Id)
            };
        }
    }
}