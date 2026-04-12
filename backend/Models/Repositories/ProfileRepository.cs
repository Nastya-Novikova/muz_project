using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;
using System.Collections.Immutable;
using backend.Models.Enums;

namespace backend.Models.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly MusicianFinderDbContext _context;

    public ProfileRepository(MusicianFinderDbContext context)
    {
        _context = context;
    }

    public async Task<(List<MusicianProfile> Items, int TotalCount)> SearchAsync(
            string? query = null,
            int? cityId = null,
            List<int>? genreIds = null,
            List<int>? specialtyIds = null,
            List<int>? goalIds = null,
            List<int>? desiredGenreIds = null,
            List<int>? desiredSpecialtyIds = null,
            LookingFor? lookingFor = null,
            ProfileType? profileType = null,
            int? experienceMin = null,
            int? experienceMax = null,
            int page = 1,
            int limit = 20,
            string? sortBy = "createdAt",
            bool sortDesc = true)
    {
        var queryable = _context.MusicianProfiles
            .Where(p => !p.IsDeleted)
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .Include(p => p.DesiredGenres)
            .Include(p => p.DesiredSpecialties)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            queryable = queryable.Where(p => p.FullName.Contains(query));

        if (cityId.HasValue)
            queryable = queryable.Where(p => p.CityId == cityId.Value);

        if (genreIds?.Count > 0)
            queryable = queryable.Where(p => p.Genres.Any(g => genreIds.Contains(g.Id)));

        if (specialtyIds?.Count > 0)
            queryable = queryable.Where(p => p.Specialties.Any(s => specialtyIds.Contains(s.Id)));

        if (goalIds?.Count > 0)
            queryable = queryable.Where(p => p.CollaborationGoals.Any(g => goalIds.Contains(g.Id)));

        if (desiredGenreIds?.Count > 0)
            queryable = queryable.Where(p => p.DesiredGenres.Any(g => desiredGenreIds.Contains(g.Id)));

        if (desiredSpecialtyIds?.Count > 0)
            queryable = queryable.Where(p => p.DesiredSpecialties.Any(s => desiredSpecialtyIds.Contains(s.Id)));

        if (lookingFor.HasValue)
            queryable = queryable.Where(p => p.LookingFor == lookingFor.Value);

        if (profileType.HasValue)
            queryable = queryable.Where(p => p.ProfileType == profileType.Value);

        if (experienceMin.HasValue)
            queryable = queryable.Where(p => p.Experience >= experienceMin.Value);
        if (experienceMax.HasValue)
            queryable = queryable.Where(p => p.Experience <= experienceMax.Value);

        var totalCount = await queryable.CountAsync();

        queryable = ApplySorting(queryable, sortBy, sortDesc);

        var items = await queryable
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<MusicianProfile?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID профиля не может быть пустым", "INVALID_PROFILE_ID");

        return await _context.MusicianProfiles
                .Include(p => p.City)
                .Include(p => p.Genres)
                .Include(p => p.Specialties)
                .Include(p => p.CollaborationGoals)
                .Include(p => p.DesiredGenres)
                .Include(p => p.DesiredSpecialties)
                .Include(p => p.AudioFiles)
                .Include(p => p.VideoFiles)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<MusicianProfile?> GetByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ApiException(400, "ID пользователя не может быть пустым", "INVALID_USER_ID");

        return await _context.MusicianProfiles
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .Include(p => p.DesiredGenres)
            .Include(p => p.DesiredSpecialties)
            .FirstOrDefaultAsync(p => !p.IsDeleted && _context.Users.Any(u => u.Id == userId && u.MusicianProfile.Id == p.Id));
    }

    public async Task AddAsync(MusicianProfile profile)
    {
        if (profile == null)
            throw new ApiException(400, "Профиль не может быть null", "PROFILE_IS_NULL");

        if (profile.Id == Guid.Empty)
            profile.Id = Guid.NewGuid();

        await _context.MusicianProfiles.AddAsync(profile);
    }

    public async Task UpdateAsync(MusicianProfile profile)
    {
        if (profile == null)
            throw new ApiException(400, "Профиль не может быть null", "PROFILE_IS_NULL");

        if (profile.Id == Guid.Empty)
            throw new ApiException(400, "ID профиля не может быть пустым", "INVALID_PROFILE_ID");

        var existing = await _context.MusicianProfiles.FindAsync(profile.Id);
        if (existing == null)
            throw new ApiException(404, "Профиль не найден", "PROFILE_NOT_FOUND");

        _context.MusicianProfiles.Update(profile);
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID профиля не может быть пустым", "INVALID_PROFILE_ID");

        var profile = await _context.MusicianProfiles.FindAsync(id);
        if (profile == null)
            throw new ApiException(404, "Профиль не найден", "PROFILE_NOT_FOUND");

        profile.IsDeleted = true;
        profile.DeletedAt = DateTime.UtcNow;
        _context.MusicianProfiles.Update(profile);
    }

    public async Task<List<MusicianProfile>> GetProfilesByIdsAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
            return new List<MusicianProfile>();

        return await _context.MusicianProfiles
            .Where(p => ids.Contains(p.Id) && !p.IsDeleted)
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .ToListAsync();
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
}