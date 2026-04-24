using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
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
                                .Contains(p.Id));

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            foreach (var dto in items)
                dto.IsFavorite = true;

            return new PagedResult<ProfileDto>
            {
                Items = items,
                Total = totalCount,
                Page = request.Page,
                Limit = request.Limit
            };
        }
    }
}