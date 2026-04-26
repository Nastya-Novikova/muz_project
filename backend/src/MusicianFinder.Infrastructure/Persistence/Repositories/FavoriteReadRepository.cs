using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Metadata;
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
        private readonly IReferenceDataReadRepository _referenceDataReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="FavoriteReadRepository"/>.
        /// </summary>
        public FavoriteReadRepository(AppDbContext dbContext, IMapper mapper, IReferenceDataReadRepository referenceDataReadRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _referenceDataReadRepository = referenceDataReadRepository;
        }

        /// <inheritdoc />
        // Infrastructure/Persistence/Repositories/FavoriteReadRepository.cs
        public async Task<PagedResult<ProfileDto>> GetFavoritesAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            // Получаем список избранных профилей
            var targetIds = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Where(p => p.Id == profileId)
                .SelectMany(p => p.Favorites.Select(f => f.TargetProfileId))
                .ToListAsync(ct);

            if (targetIds.Count == 0)
                return new PagedResult<ProfileDto> { Items = new List<ProfileDto>(), Total = 0, Page = page, Limit = limit };

            // Загружаем профили с нужными коллекциями
            var query = _dbContext.MusicianProfiles
                .AsNoTracking()
                .Include(p => p.GenreIds)
                .Include(p => p.SpecialtyIds)
                .Include(p => p.CollaborationGoalIds)
                .Include(p => p.DesiredGenreIds)
                .Include(p => p.DesiredSpecialtyIds)
                .Where(p => targetIds.Contains(p.Id) && !p.IsDeleted)
                .OrderBy(p => p.CreatedAt);  // добавлена сортировка

            var total = await query.CountAsync(ct);
            var profiles = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(ct);

            // Обогащаем справочниками
            var cities = await _referenceDataReadRepository.GetCitiesAsync(ct);
            var genres = await _referenceDataReadRepository.GetGenresAsync(ct);
            var specialties = await _referenceDataReadRepository.GetSpecialtiesAsync(ct);
            var goals = await _referenceDataReadRepository.GetCollaborationGoalsAsync(ct);

            var items = new List<ProfileDto>();
            foreach (var profile in profiles)
            {
                var dto = _mapper.Map<ProfileDto>(profile);
                dto.City = cities.FirstOrDefault(c => c.Id == profile.CityId) ?? new LookupItemDto();
                dto.Genres = genres.Where(g => profile.GenreIds.Any(gid => gid.Value == g.Id)).ToList();
                dto.Specialties = specialties.Where(s => profile.SpecialtyIds.Any(sid => sid.Value == s.Id)).ToList();
                dto.CollaborationGoals = goals.Where(cg => profile.CollaborationGoalIds.Any(cgid => cgid.Value == cg.Id)).ToList();
                dto.DesiredGenres = genres.Where(g => profile.DesiredGenreIds.Any(gid => gid.Value == g.Id)).ToList();
                dto.DesiredSpecialties = specialties.Where(s => profile.DesiredSpecialtyIds.Any(sid => sid.Value == s.Id)).ToList();
                items.Add(dto);
            }

            return new PagedResult<ProfileDto>
            {
                Items = items,
                Total = total,
                Page = page,
                Limit = limit
            };
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