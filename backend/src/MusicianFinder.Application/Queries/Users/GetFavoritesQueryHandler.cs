using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Users
{
    /// <summary>
    /// Обработчик запроса <see cref="GetFavoritesQuery"/>.
    /// </summary>
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PagedResult<ProfileDto>>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetFavoritesQueryHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="mapper">Маппер.</param>
        public GetFavoritesQueryHandler(IReadDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var query = _dbContext.Profiles
                .AsNoTracking()
                .Where(p => !p.IsDeleted &&
                            _dbContext.Users
                                .Where(u => u.Id == userId)
                                .SelectMany(u => u.Favorites)
                                .Select(f => f.ProfileId)
                                .Contains(p.Id))
                .Include(nameof(Domain.Entities.MusicianProfile.City))
                .Include(nameof(Domain.Entities.MusicianProfile.Genres))
                .Include(nameof(Domain.Entities.MusicianProfile.Specialties))
                .Include(nameof(Domain.Entities.MusicianProfile.CollaborationGoals))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredGenres))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredSpecialties))
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<ProfileDto>>(items);

            foreach (var dto in dtos)
                dto.IsFavorite = true;

            return new PagedResult<ProfileDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}