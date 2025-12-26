using System.Numerics;
using System.Text;
using System.Text.Json;
using backend.Models.Classes;
using backend.Models.DTOs;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IMusicalSpecialtyRepository _specialtyRepository;
    private readonly ICollaborationGoalRepository _goalRepository;
    private readonly ICollaborationSuggestionRepository _suggestionRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ProfileService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ICityRepository cityRepository,
        IGenreRepository genreRepository,
        IMusicalSpecialtyRepository specialtyRepository,
        ICollaborationGoalRepository goalRepository,
        ICollaborationSuggestionRepository suggestionRepository)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _cityRepository = cityRepository;
        _genreRepository = genreRepository;
        _specialtyRepository = specialtyRepository;
        _goalRepository = goalRepository;
        _suggestionRepository = suggestionRepository;
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

            var results = profiles.Select(async profile =>
            {
                return new
                {
                    profile.Id,
                    profile.FullName,
                    profile.Description,
                    profile.Phone,
                    profile.Telegram,
                    profile.City,
                    profile.Experience,
                    profile.Age,
                    profile.Avatar,
                    Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                    Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                    CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g)),
                };
            });

            var projectedResults = await Task.WhenAll(results);

            var totalPages = (int)Math.Ceiling((double)total / limit);
            var result = new
            {
                total,
                page,
                limit,
                totalPages,
                results = projectedResults
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
            if (profile == null) return null;

            profile.City = await _cityRepository.GetByIdAsync(profile.CityId);

            var result = new
            {
                profile.Id,
                profile.FullName,
                profile.Description,
                profile.Phone,
                profile.Telegram,
                profile.City,
                profile.Experience,
                profile.Age,
                profile.Avatar,
                Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g)),
                profile.Photos,
                profile.VideoFiles,
                profile.AudioFiles
            };
            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
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
            var user = await _userRepository.GetByIdAsync(userId);
            var profile = user.MusicianProfile;
            return await GetByIdAsync(profile.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByUserIdAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
                FullName = root.GetProperty("fullName").GetString() ?? "",
                Description = root.TryGetProperty("description", out var d) ? d.GetString() : null,
                Phone = root.TryGetProperty("phone", out var ph) ? ph.GetString() : null,
                Telegram = root.TryGetProperty("telegram", out var tg) ? tg.GetString() : null,
                CityId = cityId,
                Experience = root.TryGetProperty("experience", out var ex) ? ex.GetInt32() : 0,
                Age = root.TryGetProperty("age", out var a) ? a.GetInt32() : (int?)null
            };

            await _profileRepository.AddAsync(profile);

            var user = await _userRepository.GetByIdAsync(userId);
            user.MusicianProfile = profile;
            user.ProfileCreated = true;
            await _userRepository.UpdateAsync(user);

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

            await _profileRepository.UpdateAsync(profile);

            var result = new
            {
                profile.Id,
                profile.FullName,
                profile.Description,
                profile.Phone,
                profile.Telegram,
                profile.City,
                profile.Experience,
                profile.Age,
                profile.Avatar,
                Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g))
            };
            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> UpdateAsync(JsonDocument profileJson, Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return null;
            var existing = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (existing == null)
                return null;

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

            /*if (root.TryGetProperty("avatar", out var avatarElem))
                existing.Avatar = Encoding.Default.GetBytes(avatarElem.GetString());*/

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
                existing.Genres.Clear();
                existing.Genres = await _genreRepository.Genres
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();
            }

            // Обновление связей: Specialties
            if (root.TryGetProperty("specialtyIds", out var specialtyIdsElem))
            {
                var specialtyIds = specialtyIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                existing.Specialties.Clear();
                existing.Specialties = await _specialtyRepository.MusicalSpecialties
                    .Where(s => specialtyIds.Contains(s.Id))
                    .ToListAsync();
            }

            // Обновление связей: CollaborationGoals
            if (root.TryGetProperty("goalIds", out var goalIdsElem))
            {
                var goalIds = goalIdsElem.EnumerateArray().Select(x => x.GetInt32()).ToList();
                existing.CollaborationGoals.Clear();
                existing.CollaborationGoals = await _goalRepository.CollaborationGoals
                    .Where(g => goalIds.Contains(g.Id))
                    .ToListAsync();
            }

            // Обновляем дату изменения
            existing.UpdatedAt = DateTime.UtcNow;

            await _profileRepository.UpdateAsync(existing);

            var result = new
            {
                existing.Id,
                existing.FullName,
                existing.Description,
                existing.Phone,
                existing.Telegram,
                existing.City,
                existing.Experience,
                existing.Age,
                existing.Avatar,
                Genres = existing.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                Specialties = existing.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                CollaborationGoals = existing.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g)),
                existing.Photos,
                existing.VideoFiles,
                existing.AudioFiles
            };
            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
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

    



    public async Task<JsonDocument> TestSeedData()
    {
        try
        {
            JsonDocument profileJson1 = JsonDocument.Parse("{\r\n  \"fullName\": \"Алексей Иванов\",\r\n  \"age\": 28,\r\n  \"description\": \"Профессиональный гитарист, ищу группу для выступлений.\",\r\n  \"phone\": \"+79999999999\",\r\n  \"telegram\": \"null\",\r\n  \"experience\": 7,\r\n  \"cityId\": 1,\r\n  \"specialtyIds\": [1]\r\n,\r\n  \"genreIds\": [1, 2]\r\n,\r\n  \"goalIds\": [1]\r\n}");
            var profile1 = await CreateAsync(profileJson1, Guid.Parse("11111111 - 1111 - 1111 - 1111 - 111111111111"));
            JsonDocument profileJson2 = JsonDocument.Parse("{\r\n  \"fullName\": \"Иван Иванов\",\r\n  \"age\": 18,\r\n  \"description\": \"Профессиональный гитарист, ищу группу для выступлений.\",\r\n  \"phone\": \"+79999999999\",\r\n  \"telegram\": \"null\",\r\n  \"experience\": 7,\r\n  \"cityId\": 3,\r\n  \"specialtyIds\": [2, 3]\r\n,\r\n  \"genreIds\": [3, 4]\r\n,\r\n  \"goalIds\": [2, 3]\r\n}");
            var profile2 = await CreateAsync(profileJson2, Guid.Parse("22222222-2222-2222-2222-222222222222"));
            return profile2;
        }
        catch
        {
            return null;
        }
    }

   /* public async Task<JsonDocument> GetFullProfileAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (profile == null) return null;

            // Загружаем избранные профили
            var favoriteUsers = new List<User>();
            if (user?.FavoriteProfileIds != null)
            {
                favoriteUsers = await _userRepository.GetUsersByIdsAsync(user.FavoriteProfileIds);
            }

            // Получаем предложения
            var received = await _suggestionRepository.GetReceivedAsync(userId);
            var sent = await _suggestionRepository.GetSentAsync(userId);

            var extendedProfile = new
            {
                profile = new
                {
                    profile.Id,
                    profile.FullName,
                    profile.Avatar,
                    profile.Age,
                    profile.Description,
                    profile.Phone,
                    profile.Telegram,
                    profile.Experience,
                    profile.CreatedAt,
                    profile.UpdatedAt,
                    City = new { profile.City.Id, profile.City.Name, profile.City.LocalizedName },
                    Genres = profile.Genres.Select(g => new { g.Id, g.Name, g.LocalizedName }),
                    Specialties = profile.Specialties.Select(s => new { s.Id, s.Name, s.LocalizedName }),
                    CollaborationGoals = profile.CollaborationGoals.Select(g => new { g.Id, g.Name, g.LocalizedName }),
                    Media = new
                    {
                        Audio = profile.AudioFiles.Select(a => new { a.Id, a.Title, a.Description, a.MimeType, a.Duration, a.CreatedAt }),
                        Video = profile.VideoFiles.Select(v => new { v.Id, v.Title, v.Description, v.MimeType, v.Duration, v.CreatedAt }),
                        Photos = profile.Photos.Select(p => new { p.Id, p.Title, p.Description, p.MimeType, p.CreatedAt })
                    },
                    Favorites = favoriteUsers.Select(u => new
                    {
                        u.Id,
                        *//*u.FullName,
                        u.Avatar*//*
                    })
                },
                collaborations = new
                {
                    received = received ?? new List<CollaborationSuggestion>(),
                    sent = sent ?? new List<CollaborationSuggestion>()
                },
                favorites = favoriteUsers.Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.CreatedAt,
                    u.FavoriteProfileIds,
                    u.MusicianProfile,
                    u.IsDeleted,
                    u.DeletedAt
                })
            };

            return JsonDocument.Parse(JsonSerializer.Serialize(extendedProfile, _options));
        }
        catch
        {
            return null;
        }
    }*/

    /*public async Task<JsonDocument?> GetFullProfileAsync(Guid userId)
    {
        try
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null) return null;

            // Загружаем избранные профили
            var favoriteUsers = new List<User>();
            if (profile.User?.FavoriteProfileIds != null)
            {
                favoriteUsers = await _userRepository.GetUsersByIdsAsync(profile.User.FavoriteProfileIds);
            }

            var received = await _suggestionRepository.GetReceivedAsync(userId);
            var sent = await _suggestionRepository.GetSentAsync(userId);

            // Формируем расширенный профиль
            var extendedProfile = new
            {
                profile.Id,
                profile.UserId,
                profile.FullName,
                profile.Avatar,
                profile.Age,
                profile.Description,
                profile.Phone,
                profile.Telegram,
                profile.Experience,
                profile.CreatedAt,
                profile.UpdatedAt,
                City = new { profile.City.Id, profile.City.Name, profile.City.LocalizedName },
                Genres = profile.Genres.Select(g => new { g.Id, g.Name, g.LocalizedName }),
                Specialties = profile.Specialties.Select(s => new { s.Id, s.Name, s.LocalizedName }),
                CollaborationGoals = profile.CollaborationGoals.Select(g => new { g.Id, g.Name, g.LocalizedName }),
                Media = new
                {
                    Audio = profile.AudioFiles.Select(a => new { a.Id, a.Title, a.Description, a.MimeType, a.Duration, a.CreatedAt }),
                    Video = profile.VideoFiles.Select(v => new { v.Id, v.Title, v.Description, v.MimeType, v.Duration, v.CreatedAt }),
                    Photos = profile.Photos.Select(p => new { p.Id, p.Title, p.Description, p.MimeType, p.CreatedAt })
                },
                Collaborations = new
                {
                    received,
                    sent
                },
                Favorites = favoriteUsers.Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FullName,
                    u.Avatar,
                    u.ProfileCompleted,
                    u.CreatedAt,
                    u.FavoriteProfileIds,
                    u.MusicianProfile,
                    u.IsDeleted,
                    u.DeletedAt
                })
            };

            return JsonDocument.Parse(JsonSerializer.Serialize(extendedProfile, _options));
        }
        catch
        {
            return null;
        }
    }*/
}