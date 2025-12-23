using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<MusicianProfile> MusicianProfiles { get; set; }

    public ProfileRepository(MusicianFinderDbContext context)
    {
        _context = context;
        MusicianProfiles = _context.Set<MusicianProfile>();
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
            .Include(p => p.User)
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .AsQueryable();

        // Фильтрация по строке
        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(p => p.FullName.Contains(query));
        }

        // Фильтрация по городу
        if (cityId.HasValue)
        {
            queryable = queryable.Where(p => p.CityId == cityId.Value);
        }

        // Фильтрация по жанрам
        if (genreIds?.Count > 0)
        {
            queryable = queryable.Where(p => p.Genres.Any(g => genreIds.Contains(g.Id)));
        }

        // Фильтрация по специальностям
        if (specialtyIds?.Count > 0)
        {
            queryable = queryable.Where(p => p.Specialties.Any(s => specialtyIds.Contains(s.Id)));
        }

        // Фильтрация по целям
        if (goalIds?.Count > 0)
        {
            queryable = queryable.Where(p => p.CollaborationGoals.Any(g => goalIds.Contains(g.Id)));
        }

        // Фильтрация по опыту
        if (experienceMin.HasValue)
        {
            queryable = queryable.Where(p => p.Experience >= experienceMin.Value);
        }
        if (experienceMax.HasValue)
        {
            queryable = queryable.Where(p => p.Experience <= experienceMax.Value);
        }

        // Общее количество (до пагинации)
        var total = await queryable.CountAsync();

        // Сортировка
        queryable = ApplySorting(queryable, sortBy, sortDesc);

        // Пагинация
        var profiles = await queryable
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (profiles, total);
    }

    public async Task<MusicianProfile?> GetByIdAsync(Guid id)
    {
        return await MusicianProfiles
            .Include(p => p.User)
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .Include(p => p.AudioFiles)
            .Include(p => p.VideoFiles)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<MusicianProfile?> GetByUserIdAsync(Guid userId)
    {
        return await MusicianProfiles
            .Include(p => p.User)
            .Include(p => p.City)
            .Include(p => p.Genres)
            .Include(p => p.Specialties)
            .Include(p => p.CollaborationGoals)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task AddAsync(MusicianProfile profile)
    {
        await MusicianProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MusicianProfile profile)
    {
        MusicianProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var profile = await MusicianProfiles.FindAsync(id);
        if (profile != null)
        {
            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;
            MusicianProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }
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