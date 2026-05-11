using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Metadata;
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
        private readonly IReferenceDataReadRepository _referenceRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ProfileReadRepository"/>.
        /// </summary>
        public ProfileReadRepository(AppDbContext dbContext, IMapper mapper, IReferenceDataReadRepository referenceRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _referenceRepository = referenceRepository;
        }

        /// <inheritdoc />
        public async Task<ProfileDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Include(p => p.GenreIds)
                .Include(p => p.SpecialtyIds)
                .Include(p => p.CollaborationGoalIds)
                .Include(p => p.DesiredGenreIds)
                .Include(p => p.DesiredSpecialtyIds)
                .Include(p => p.Portfolio)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

            return profile == null ? null : await EnrichProfileDtoAsync(profile, ct);
        }

        /// <inheritdoc />
        public async Task<ProfileDto?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var profile = await _dbContext.MusicianProfiles
                .AsNoTracking()
                .Include(p => p.GenreIds)
                .Include(p => p.SpecialtyIds)
                .Include(p => p.CollaborationGoalIds)
                .Include(p => p.DesiredGenreIds)
                .Include(p => p.DesiredSpecialtyIds)
                .Include(p => p.Portfolio)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, ct);

            return profile == null ? null : await EnrichProfileDtoAsync(profile, ct);
        }

        /// <inheritdoc />
        public async Task<PagedResult<ProfileDto>> SearchAsync(SearchProfilesQuery query, CancellationToken ct = default)
        {
            var entityQuery = _dbContext.MusicianProfiles
                .AsNoTracking()
                .Include(p => p.GenreIds)
                .Include(p => p.SpecialtyIds)
                .Include(p => p.CollaborationGoalIds)
                .Include(p => p.DesiredGenreIds)
                .Include(p => p.DesiredSpecialtyIds)
                .AsSplitQuery()
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Query))
            {
                var search = query.Query.ToLower();
                entityQuery = entityQuery.Where(p =>
                    p.FullName.Value.ToLower().Contains(search) ||
                    (p.Description != null && p.Description.ToLower().Contains(search)));
            }

            if (query.CityId.HasValue)
                entityQuery = entityQuery.Where(p => p.CityId == query.CityId.Value);

            if (query.GenreIds != null && query.GenreIds.Any())
                entityQuery = entityQuery.Where(p => p.GenreIds.Any(g => query.GenreIds.Contains(g.Value)));

            if (query.SpecialtyIds != null && query.SpecialtyIds.Any())
                entityQuery = entityQuery.Where(p => p.SpecialtyIds.Any(s => query.SpecialtyIds.Contains(s.Value)));

            if (query.GoalIds != null && query.GoalIds.Any())
                entityQuery = entityQuery.Where(p => p.CollaborationGoalIds.Any(g => query.GoalIds.Contains(g.Value)));

            if (query.DesiredGenreIds != null && query.DesiredGenreIds.Any())
                entityQuery = entityQuery.Where(p => p.DesiredGenreIds.Any(g => query.DesiredGenreIds.Contains(g.Value)));

            if (query.DesiredSpecialtyIds != null && query.DesiredSpecialtyIds.Any())
                entityQuery = entityQuery.Where(p => p.DesiredSpecialtyIds.Any(s => query.DesiredSpecialtyIds.Contains(s.Value)));

            if (!string.IsNullOrWhiteSpace(query.LookingFor))
                entityQuery = entityQuery.Where(p => p.LookingFor == Enum.Parse<Domain.Enums.LookingFor>(query.LookingFor));

            if (!string.IsNullOrWhiteSpace(query.ProfileType))
                entityQuery = entityQuery.Where(p => p.ProfileType == Enum.Parse<Domain.Enums.ProfileType>(query.ProfileType));

            if (query.ExperienceMin.HasValue)
                entityQuery = entityQuery.Where(p => p.Experience >= query.ExperienceMin.Value);

            if (query.ExperienceMax.HasValue)
                entityQuery = entityQuery.Where(p => p.Experience <= query.ExperienceMax.Value);

            var totalCount = await entityQuery.CountAsync(ct);

            var profiles = await entityQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync(ct);

            var items = new List<ProfileDto>(profiles.Count);
            foreach (var profile in profiles)
                items.Add(await EnrichProfileDtoAsync(profile, ct));

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

        /// <summary>
        /// Обогащает ProfileDto данными из справочников: город, жанры, специальности, цели.
        /// </summary>
        private async Task<ProfileDto> EnrichProfileDtoAsync(MusicianProfile profile, CancellationToken ct)
        {
            var dto = _mapper.Map<ProfileDto>(profile);

            var cities = await _referenceRepository.GetCitiesAsync(ct);
            var genres = await _referenceRepository.GetGenresAsync(ct);
            var specialties = await _referenceRepository.GetSpecialtiesAsync(ct);
            var goals = await _referenceRepository.GetCollaborationGoalsAsync(ct);

            dto.City = cities.FirstOrDefault(c => c.Id == profile.CityId) ?? new LookupItemDto();

            dto.Genres = genres.Where(g => profile.GenreIds.Any(gid => gid.Value == g.Id)).ToList();
            dto.Specialties = specialties.Where(s => profile.SpecialtyIds.Any(sid => sid.Value == s.Id)).ToList();
            dto.CollaborationGoals = goals.Where(cg => profile.CollaborationGoalIds.Any(cgid => cgid.Value == cg.Id)).ToList();
            dto.DesiredGenres = genres.Where(g => profile.DesiredGenreIds.Any(gid => gid.Value == g.Id)).ToList();
            dto.DesiredSpecialties = specialties.Where(s => profile.DesiredSpecialtyIds.Any(sid => sid.Value == s.Id)).ToList();

            var items = profile.Portfolio.ToList();
            dto.Audio = items.Where(x => x.Type == Domain.Enums.MediaType.Audio)
                             .Select(i => _mapper.Map<AudioDto>(i)).ToList();
            dto.Video = items.Where(x => x.Type == Domain.Enums.MediaType.Video)
                             .Select(i => _mapper.Map<VideoDto>(i)).ToList();
            dto.Photos = items.Where(x => x.Type == Domain.Enums.MediaType.Photo)
                              .Select(i => _mapper.Map<PhotoDto>(i)).ToList();

            return dto;
        }
    }
}