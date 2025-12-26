using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<MusicianProfile> MusicianProfiles { get; set; }

    private readonly ICityRepository _cityRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IMusicalSpecialtyRepository _musicalSpecialtyRepository;
    private readonly ICollaborationGoalRepository _collaborationGoalRepository;

    public ProfileRepository(MusicianFinderDbContext context, ICityRepository cityRepository, IMusicalSpecialtyRepository musicalSpecialtyRepository, ICollaborationGoalRepository collaborationGoalRepository, IGenreRepository genreRepository)
    {
        _context = context;
        MusicianProfiles = _context.Set<MusicianProfile>();
        _cityRepository = cityRepository;
        _musicalSpecialtyRepository = musicalSpecialtyRepository;
        _collaborationGoalRepository = collaborationGoalRepository;
        _genreRepository = genreRepository;
    }

    public async Task<(List<MusicianProfile> Profiles, int Total)> SearchAsync(
        string? query = null,
        int? cityId = null,
        List<int>? genreIds = null,
        List<int>? specialtyIds = null,
        List<int>? goalIds = null,
        int? experienceMin = null,
        int? experienceMax = null,
        int page = 1,
        int limit = 20,
        string? sortBy = "createdAt",
        bool sortDesc = true)
    {
        var queryable = MusicianProfiles
            /*.Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)*/
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(p => p.FullName.Contains(query));
        }

        if (cityId.HasValue)
        {
            queryable = queryable.Where(p => p.CityId == cityId.Value);
        }

        if (genreIds?.Count > 0)
        {
            queryable = queryable.Where(p => p.Genres.Any(g => genreIds.Contains(g.Id)));
        }

        if (specialtyIds?.Count > 0)
        {
            queryable = queryable.Where(p => p.Specialties.Any(s => specialtyIds.Contains(s.Id)));
        }

        if (goalIds?.Count > 0)
        {
            queryable = queryable.Where(p => p.CollaborationGoals.Any(g => goalIds.Contains(g.Id)));
        }

        if (experienceMin.HasValue)
        {
            queryable = queryable.Where(p => p.Experience >= experienceMin.Value);
        }
        if (experienceMax.HasValue)
        {
            queryable = queryable.Where(p => p.Experience <= experienceMax.Value);
        }

        var total = await queryable.CountAsync();

        queryable = ApplySorting(queryable, sortBy, sortDesc);

        var profiles = await queryable
            .Skip((page - 1) * limit)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .Include(p => p.City)
            .Take(limit)
            .ToListAsync();

        return (profiles, total);
    }

    public async Task<MusicianProfile?> GetByIdAsync(Guid id)
    {

        return await MusicianProfiles
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .Include(p => p.AudioFiles)
            .Include(p => p.VideoFiles)
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task AddAsync(MusicianProfile profile)
    {
        var city = await _cityRepository.GetByIdAsync(profile.CityId);
        if (city == null)
            throw new ApiException(404, "Город не найден", "CITY_NOT_FOUND");

        foreach (var genre in profile.Genres)
        {
            var existingGenre = await _genreRepository.GetByIdAsync(genre.Id);
            if (existingGenre != null && !existingGenre.Profiles.Any(p => p.Id == profile.Id))
            {
                existingGenre.Profiles.Add(profile);
            }
        }

        foreach (var specialty in profile.Specialties)
        {
            var existingSpecialty = await _musicalSpecialtyRepository.GetByIdAsync(specialty.Id);
            if (existingSpecialty != null && !existingSpecialty.Profiles.Any(p => p.Id == profile.Id))
            {
                existingSpecialty.Profiles.Add(profile);
            }
        }

        foreach (var goal in profile.CollaborationGoals)
        {
            var existingGoal = await _collaborationGoalRepository.GetByIdAsync(goal.Id);
            if (existingGoal != null && !existingGoal.Profiles.Any(p => p.Id == profile.Id))
            {
                existingGoal.Profiles.Add(profile);
            }
        }

        await MusicianProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MusicianProfile profile)
    {
        if (profile.Id == Guid.Empty)
            throw new ApiException(400, "Некорректный ID профиля", "INVALID_PROFILE_ID");

        MusicianProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var profile = await MusicianProfiles.FindAsync(id);
        if (profile == null)
            throw new ApiException(404, "Профиль не найден", "PROFILE_NOT_FOUND");

        profile.IsDeleted = true;
        profile.DeletedAt = DateTime.UtcNow;
        MusicianProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    private static IQueryable<MusicianProfile> ApplySorting(IQueryable<MusicianProfile> query, string? sortBy, bool sortDesc)
    {
        return sortBy?.ToLower() switch
        {
            "fullname" => sortDesc ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
            "age" => sortDesc ? query.OrderByDescending(p => p.Age) : query.OrderBy(p => p.Age),
            "experience" => sortDesc ? query.OrderByDescending(p => p.Experience) : query.OrderBy(p => p.Experience),
            "city" => sortDesc ? query.OrderByDescending(p => p.City.Name) : query.OrderBy(p => p.City.Name),
            "createdat" => sortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };
    }

    public async Task<List<MusicianProfile>> GetProfilesByIdsAsync(List<Guid> ids)
    {
        return await MusicianProfiles.Where(u => ids.Contains(u.Id) && !u.IsDeleted).ToListAsync();
    }
}