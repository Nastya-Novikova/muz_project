using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Profiles;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для профилей музыкантов.
    /// </summary>
    public class ProfileReadRepository : IProfileReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ProfileReadRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public ProfileReadRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<ProfileDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.Id == id && !p.IsDeleted)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        /// <inheritdoc />
        public async Task<ProfileDto?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> SearchAsync(SearchProfilesQuery query, CancellationToken ct = default)
        {
            var entityQuery = _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(query.Query))
                entityQuery = entityQuery.Where(p => p.FullName.Value.Contains(query.Query));

            if (query.CityId.HasValue)
                entityQuery = entityQuery.Where(p => p.CityId == query.CityId.Value);

            if (query.ExperienceMin.HasValue)
                entityQuery = entityQuery.Where(p => p.Experience >= query.ExperienceMin.Value);

            if (query.ExperienceMax.HasValue)
                entityQuery = entityQuery.Where(p => p.Experience <= query.ExperienceMax.Value);

            var totalCount = await entityQuery.CountAsync(ct);

            var items = await entityQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<ProfileDto>
            {
                Items = items,
                Total = totalCount,
                Page = query.Page,
                Limit = query.Limit
            };
        }

        /// <inheritdoc />
        public async Task<MediaDto?> GetMediaAsync(Guid profileId, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Include(nameof(MusicianProfile.Portfolio))
                .FirstOrDefaultAsync(p => p.Id == profileId && !p.IsDeleted, ct);

            if (profile == null) return null;

            var items = profile.Portfolio.ToList();
            return new MediaDto
            {
                Audio = _mapper.Map<List<AudioDto>>(items.Where(x => x.Type == Domain.Enums.MediaType.Audio).ToList()),
                Video = _mapper.Map<List<VideoDto>>(items.Where(x => x.Type == Domain.Enums.MediaType.Video).ToList()),
                Photos = _mapper.Map<List<PhotoDto>>(items.Where(x => x.Type == Domain.Enums.MediaType.Photo).ToList())
            };
        }
    }
}