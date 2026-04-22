using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Обработчик запроса <see cref="SearchProfilesQuery"/>.
    /// </summary>
    public class SearchProfilesQueryHandler : IRequestHandler<SearchProfilesQuery, PagedResult<ProfileDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SearchProfilesQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public SearchProfilesQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> Handle(SearchProfilesQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Profiles
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Include(nameof(MusicianProfile.City))
                .Include(nameof(MusicianProfile.Genres))
                .Include(nameof(MusicianProfile.Specialties))
                .Include(nameof(MusicianProfile.CollaborationGoals))
                .Include(nameof(MusicianProfile.DesiredGenres))
                .Include(nameof(MusicianProfile.DesiredSpecialties))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Query))
                query = query.Where(p => p.FullName.Contains(request.Query));

            if (request.CityId.HasValue)
                query = query.Where(p => p.CityId == request.CityId.Value);

            if (request.GenreIds?.Count > 0)
                query = query.Where(p => p.Genres.Any(g => request.GenreIds.Contains(g.Id)));

            if (request.SpecialtyIds?.Count > 0)
                query = query.Where(p => p.Specialties.Any(s => request.SpecialtyIds.Contains(s.Id)));

            if (request.GoalIds?.Count > 0)
                query = query.Where(p => p.CollaborationGoals.Any(g => request.GoalIds.Contains(g.Id)));

            if (request.DesiredGenreIds?.Count > 0)
                query = query.Where(p => p.DesiredGenres.Any(g => request.DesiredGenreIds.Contains(g.Id)));

            if (request.DesiredSpecialtyIds?.Count > 0)
                query = query.Where(p => p.DesiredSpecialties.Any(s => request.DesiredSpecialtyIds.Contains(s.Id)));

            if (request.LookingFor.HasValue)
                query = query.Where(p => p.LookingFor == request.LookingFor.Value);

            if (request.ProfileType.HasValue)
                query = query.Where(p => p.ProfileType == request.ProfileType.Value);

            if (request.ExperienceMin.HasValue)
                query = query.Where(p => p.Experience >= request.ExperienceMin.Value);

            if (request.ExperienceMax.HasValue)
                query = query.Where(p => p.Experience <= request.ExperienceMax.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(query, request.SortBy, request.SortDesc);

            var items = await query
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<ProfileDto>>(items);

            if (_currentUserService.IsAuthenticated)
            {
                var currentProfile = await _dbContext.Profiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken);

                if (currentProfile != null)
                {
                    var favoriteIds = await _dbContext.Users
                        .Where(u => u.Id == _currentUserService.UserId)
                        .SelectMany(u => u.Favorites)
                        .Select(f => f.ProfileId)
                        .ToListAsync(cancellationToken);

                    foreach (var dto in dtos)
                    {
                        dto.IsMyProfile = dto.Id == currentProfile.Id;
                        dto.IsFavorite = favoriteIds.Contains(dto.Id);
                    }
                }
            }

            return new PagedResult<ProfileDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }

        private static IQueryable<MusicianProfile> ApplySorting(IQueryable<MusicianProfile> query, string? sortBy, bool sortDesc)
        {
            return sortBy?.ToLower() switch
            {
                "fullname" => sortDesc ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                "age" => sortDesc ? query.OrderByDescending(p => p.Age) : query.OrderBy(p => p.Age),
                "experience" => sortDesc ? query.OrderByDescending(p => p.Experience) : query.OrderBy(p => p.Experience),
                "city" => sortDesc ? query.OrderByDescending(p => p.City != null ? p.City.Name : string.Empty) : query.OrderBy(p => p.City != null ? p.City.Name : string.Empty),
                "createdat" => sortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
        }
    }
}