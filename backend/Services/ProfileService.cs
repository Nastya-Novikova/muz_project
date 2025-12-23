using System.Text.Json;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IMusicalSpecialtyRepository _specialtyRepository;
    private readonly ICollaborationGoalRepository _goalRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ProfileService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ICityRepository cityRepository,
        IGenreRepository genreRepository,
        IMusicalSpecialtyRepository specialtyRepository,
        ICollaborationGoalRepository goalRepository)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _cityRepository = cityRepository;
        _genreRepository = genreRepository;
        _specialtyRepository = specialtyRepository;
        _goalRepository = goalRepository;
    }

    public async Task<JsonDocument?> SearchAsync(JsonDocument searchParams)
    {
        try
        {
            var root = searchParams.RootElement;
            var query = root.TryGetProperty("query", out var q) ? q.GetString() : null;
            var cityId = root.TryGetProperty("cityId", out var c) ? c.GetInt32() : (int?)null;
            var experienceMin = root.TryGetProperty("experienceMin", out var emin) ? emin.GetInt32() : (int?)null;
            var experienceMax = root.TryGetProperty("experienceMax", out var emax) ? emax.GetInt32() : (int?)null;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;
            var sortBy = root.TryGetProperty("sortBy", out var sb) ? sb.GetString() : "createdAt";
            var sortDesc = root.TryGetProperty("sortDesc", out var sd) ? sd.GetBoolean() : true;

            // Извлечение списков
            var genreIds = root.TryGetProperty("genreIds", out var genres)
                ? genres.EnumerateArray().Select(x => x.GetInt32()).ToList()
                : null;
            var specialtyIds = root.TryGetProperty("specialtyIds", out var specialties)
                ? specialties.EnumerateArray().Select(x => x.GetInt32()).ToList()
                : null;
            var goalIds = root.TryGetProperty("goalIds", out var goals)
                ? goals.EnumerateArray().Select(x => x.GetInt32()).ToList()
                : null;

            var (profiles, total) = await _profileRepository.SearchAsync(
                query, cityId, genreIds, specialtyIds, goalIds,
                experienceMin, experienceMax,
                page, limit, sortBy, sortDesc);

            var totalPages = (int)Math.Ceiling((double)total / limit);
            var result = new
            {
                total,
                page,
                limit,
                totalPages,
                results = profiles
            };

            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetByIdAsync(Guid id)
    {
        try
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            return profile == null ? null : JsonDocument.Parse(JsonSerializer.Serialize(profile, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            return profile == null ? null : JsonDocument.Parse(JsonSerializer.Serialize(profile, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> CreateAsync(JsonDocument profileJson, Guid userId)
    {
        try
        {
            var root = profileJson.RootElement;
            if (string.IsNullOrWhiteSpace(root.GetProperty("fullName").GetString()))
                return null;

            // Валидация связей
            var cityId = root.GetProperty("cityId").GetInt32();
            if (await _cityRepository.GetByIdAsync(cityId) == null) return null;

            var profile = new MusicianProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FullName = root.GetProperty("fullName").GetString() ?? "",
                Description = root.TryGetProperty("description", out var d) ? d.GetString() : null,
                Phone = root.TryGetProperty("phone", out var ph) ? ph.GetString() : null,
                Telegram = root.TryGetProperty("telegram", out var tg) ? tg.GetString() : null,
                CityId = cityId,
                Experience = root.TryGetProperty("experience", out var ex) ? ex.GetInt32() : 0,
                Age = root.TryGetProperty("age", out var a) ? a.GetInt32() : (int?)null
            };

            // Загрузка связей
            if (root.TryGetProperty("genreIds", out var genreIdsElem))
            {
                var genreIds = genreIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                profile.Genres = (await _genreRepository.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync());
            }
            if (root.TryGetProperty("specialtyIds", out var specialtyIdsElem))
            {
                var specialtyIds = specialtyIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                profile.Specialties = (await _specialtyRepository.MusicalSpecialties.Where(s => specialtyIds.Contains(s.Id)).ToListAsync());
            }
            if (root.TryGetProperty("goalIds", out var goalIdsElem))
            {
                var goalIds = goalIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                profile.CollaborationGoals = (await _goalRepository.CollaborationGoals.Where(g => goalIds.Contains(g.Id)).ToListAsync());
            }

            await _profileRepository.AddAsync(profile);
            return JsonDocument.Parse(JsonSerializer.Serialize(profile, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> UpdateAsync(Guid id, JsonDocument profileJson, Guid userId)
    {
        try
        {
            var existing = await _profileRepository.GetByIdAsync(id);
            if (existing == null || existing.UserId != userId)
                return null; // Профиль не найден или не принадлежит пользователю

            var root = profileJson.RootElement;

            // Обновляем только указанные поля
            if (root.TryGetProperty("fullName", out var fullNameElem) && !string.IsNullOrWhiteSpace(fullNameElem.GetString()))
                existing.FullName = fullNameElem.GetString()!;

            if (root.TryGetProperty("description", out var descElem))
                existing.Description = descElem.GetString();

            if (root.TryGetProperty("phone", out var phoneElem))
                existing.Phone = phoneElem.GetString();

            if (root.TryGetProperty("telegram", out var telegramElem))
                existing.Telegram = telegramElem.GetString();

            if (root.TryGetProperty("age", out var ageElem))
                existing.Age = ageElem.GetInt32();

            // Обновление города с валидацией
            if (root.TryGetProperty("cityId", out var cityIdElem))
            {
                var cityId = cityIdElem.GetInt32();
                if (await _cityRepository.GetByIdAsync(cityId) == null)
                    return null;
                existing.CityId = cityId;
            }

            if (root.TryGetProperty("experience", out var expElem))
                existing.Experience = expElem.GetInt32();

            // Обновление связей: Genres
            if (root.TryGetProperty("genreIds", out var genreIdsElem))
            {
                var genreIds = genreIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                existing.Genres = await _genreRepository.Genres
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();
            }

            // Обновление связей: Specialties
            if (root.TryGetProperty("specialtyIds", out var specialtyIdsElem))
            {
                var specialtyIds = specialtyIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                existing.Specialties = await _specialtyRepository.MusicalSpecialties
                    .Where(s => specialtyIds.Contains(s.Id))
                    .ToListAsync();
            }

            // Обновление связей: CollaborationGoals
            if (root.TryGetProperty("goalIds", out var goalIdsElem))
            {
                var goalIds = goalIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                existing.CollaborationGoals = await _goalRepository.CollaborationGoals
                    .Where(g => goalIds.Contains(g.Id))
                    .ToListAsync();
            }

            // Обновляем дату изменения
            existing.UpdatedAt = DateTime.UtcNow;

            await _profileRepository.UpdateAsync(existing);
            return JsonDocument.Parse(JsonSerializer.Serialize(existing, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _profileRepository.GetByIdAsync(id);
            if (existing == null) return null;
            await _profileRepository.SoftDeleteAsync(id);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }
}