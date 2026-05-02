using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для предложений о сотрудничестве.
    /// </summary>
    public class CollaborationSuggestionReadRepository : ICollaborationSuggestionReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IReferenceDataReadRepository _referenceRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProfileReadRepository _profileReadRepository;
        private readonly IFavoriteReadRepository _favoriteReadRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CollaborationSuggestionReadRepository"/>.
        /// </summary>
        public CollaborationSuggestionReadRepository(AppDbContext dbContext, IMapper mapper, IReferenceDataReadRepository referenceRepo, ICurrentUserService currentUserService, IProfileReadRepository profileReadRepository, IFavoriteReadRepository favoriteReadRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _referenceRepo = referenceRepo;
            _currentUserService = currentUserService;
            _profileReadRepository = profileReadRepository;
            _favoriteReadRepository = favoriteReadRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> GetReceivedAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            var query = _dbContext.CollaborationSuggestions
                .AsNoTracking()
                .Include(cs => cs.FromProfile)
                    .ThenInclude(p => p.GenreIds)
                .Include(cs => cs.FromProfile)
                    .ThenInclude(p => p.SpecialtyIds)
                .Include(cs => cs.FromProfile)
                    .ThenInclude(p => p.CollaborationGoalIds)
                .Include(cs => cs.FromProfile)
                    .ThenInclude(p => p.DesiredGenreIds)
                .Include(cs => cs.FromProfile)
                    .ThenInclude(p => p.DesiredSpecialtyIds)
                .Where(cs => cs.ToProfileId == profileId && cs.Status != Domain.Enums.SuggestionStatus.Rejected && cs.Status != Domain.Enums.SuggestionStatus.Withdrawn)
                .OrderByDescending(cs => cs.CreatedAt);

            var total = await query.CountAsync(ct);
            var suggestions = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(ct);

            var items = new List<SuggestionDto>(suggestions.Count);
            foreach (var s in suggestions)
            {
                items.Add(new SuggestionDto
                {
                    Id = s.Id,
                    Message = s.Message,
                    Status = s.Status.ToString(),
                    CreatedAt = s.CreatedAt,
                    FromProfile = await BuildProfileDtoAsync(s.FromProfile, ct),
                    ToProfile = null
                });
            }

            await PopulateProfileFlags(items, ct);

            return new PagedResult<SuggestionDto> { Items = items, Total = total, Page = page, Limit = limit };
        }

        /// <inheritdoc />
        public async Task<PagedResult<SuggestionDto>> GetSentAsync(Guid profileId, int page, int limit, CancellationToken ct)
        {
            var query = _dbContext.CollaborationSuggestions
                .AsNoTracking()
                .Include(cs => cs.ToProfile)
                    .ThenInclude(p => p.GenreIds)
                .Include(cs => cs.ToProfile)
                    .ThenInclude(p => p.SpecialtyIds)
                .Include(cs => cs.ToProfile)
                    .ThenInclude(p => p.CollaborationGoalIds)
                .Include(cs => cs.ToProfile)
                    .ThenInclude(p => p.DesiredGenreIds)
                .Include(cs => cs.ToProfile)
                    .ThenInclude(p => p.DesiredSpecialtyIds)
                .Where(cs => cs.FromProfileId == profileId && cs.Status != Domain.Enums.SuggestionStatus.Rejected && cs.Status != Domain.Enums.SuggestionStatus.Withdrawn)
                .OrderByDescending(cs => cs.CreatedAt);

            var total = await query.CountAsync(ct);
            var suggestions = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(ct);

            var items = new List<SuggestionDto>(suggestions.Count);
            foreach (var s in suggestions)
            {
                items.Add(new SuggestionDto
                {
                    Id = s.Id,
                    Message = s.Message,
                    Status = s.Status.ToString(),
                    CreatedAt = s.CreatedAt,
                    ToProfile = await BuildProfileDtoAsync(s.ToProfile, ct),
                    FromProfile = null
                });
            }

            await PopulateProfileFlags(items, ct);

            return new PagedResult<SuggestionDto> { Items = items, Total = total, Page = page, Limit = limit };
        }

        /// <inheritdoc />
        public async Task<HashSet<Guid>> GetSentSuggestionToProfileIdsAsync(Guid fromProfileId, IEnumerable<Guid> toProfileIds, CancellationToken ct)
        {
            var ids = await _dbContext.CollaborationSuggestions
                .AsNoTracking()
                .Where(cs => cs.FromProfileId == fromProfileId
                            && toProfileIds.Contains(cs.ToProfileId)
                            && cs.Status != Domain.Enums.SuggestionStatus.Rejected
                            && cs.Status != Domain.Enums.SuggestionStatus.Withdrawn)
                .Select(cs => cs.ToProfileId)
                .Distinct()
                .ToListAsync(ct);

            return new HashSet<Guid>(ids);
        }

        private async Task<ProfileDto> BuildProfileDtoAsync(MusicianProfile profile, CancellationToken ct)
        {
            var dto = _mapper.Map<ProfileDto>(profile);

            var cities = await _referenceRepo.GetCitiesAsync(ct);
            var genres = await _referenceRepo.GetGenresAsync(ct);
            var specialties = await _referenceRepo.GetSpecialtiesAsync(ct);
            var goals = await _referenceRepo.GetCollaborationGoalsAsync(ct);

            dto.City = cities.FirstOrDefault(c => c.Id == profile.CityId) ?? new LookupItemDto();

            dto.Genres = genres
                .Where(g => profile.GenreIds.Select(x => x.Value).Contains(g.Id))
                .ToList();
            dto.Specialties = specialties
                .Where(s => profile.SpecialtyIds.Select(x => x.Value).Contains(s.Id))
                .ToList();
            dto.CollaborationGoals = goals
                .Where(g => profile.CollaborationGoalIds.Select(x => x.Value).Contains(g.Id))
                .ToList();
            dto.DesiredGenres = genres
                .Where(g => profile.DesiredGenreIds.Select(x => x.Value).Contains(g.Id))
                .ToList();
            dto.DesiredSpecialties = specialties
                .Where(s => profile.DesiredSpecialtyIds.Select(x => x.Value).Contains(s.Id))
                .ToList();

            return dto;
        }

        private async Task PopulateProfileFlags(List<SuggestionDto> items, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                return;

            var currentProfile = await _profileReadRepository.GetByUserIdAsync(_currentUserService.UserId, ct);
            if (currentProfile == null)
                return;

            // Собрать все ProfileId из FromProfile и ToProfile
            var profileIds = new HashSet<Guid>();
            foreach (var item in items)
            {
                if (item.FromProfile != null) profileIds.Add(item.FromProfile.Id);
                if (item.ToProfile != null) profileIds.Add(item.ToProfile.Id);
            }

            // Получить избранные и сотрудничающие профили одним запросом
            var favoritedIds = await _favoriteReadRepository.GetFavoritedProfileIdsAsync(currentProfile.Id, profileIds, ct);
            var sentSuggestionIds = await _dbContext.CollaborationSuggestions
                .Where(cs => cs.FromProfileId == currentProfile.Id && profileIds.Contains(cs.ToProfileId))
                .Select(cs => cs.ToProfileId)
                .Distinct()
                .ToListAsync(ct);
            var collaboratedIds = new HashSet<Guid>(sentSuggestionIds);

            foreach (var item in items)
            {
                if (item.FromProfile != null)
                {
                    item.FromProfile.IsMyProfile = item.FromProfile.Id == currentProfile.Id;
                    item.FromProfile.IsFavorite = favoritedIds.Contains(item.FromProfile.Id);
                    item.FromProfile.IsCollaborated = collaboratedIds.Contains(item.FromProfile.Id);
                }
                if (item.ToProfile != null)
                {
                    item.ToProfile.IsMyProfile = item.ToProfile.Id == currentProfile.Id;
                    item.ToProfile.IsFavorite = favoritedIds.Contains(item.ToProfile.Id);
                    item.ToProfile.IsCollaborated = collaboratedIds.Contains(item.ToProfile.Id);
                }
            }
        }
    }
}