using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для избранных профилей.
    /// </summary>
    public class FavoriteReadRepository : IFavoriteReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FavoriteReadRepository"/>.
        /// </summary>
        public FavoriteReadRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> GetFavoritesAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            var query = _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.Favorites.Any(f => f.TargetProfileId == profileId));

            var total = await query.CountAsync(ct);

            var items = await query.Skip((page - 1) * limit).Take(limit)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<ProfileDto> { Items = items, Total = total, Page = page, Limit = limit };
        }

        /// <inheritdoc />
        public async Task<HashSet<Guid>> GetFavoritedProfileIdsAsync(Guid addedByProfileId, IEnumerable<Guid> targetProfileIds, CancellationToken ct)
        {
            var ids = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.Id == addedByProfileId)
                .SelectMany(p => p.Favorites)
                .Where(f => targetProfileIds.Contains(f.TargetProfileId))
                .Select(f => f.TargetProfileId)
                .Distinct()
                .ToListAsync(ct);

            return new HashSet<Guid>(ids);
        }
    }
}