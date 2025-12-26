using System.Text.Json;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public FavoriteService(IUserRepository userRepository, IProfileRepository profileRepository)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    public async Task<JsonDocument?> AddAsync(Guid userId, Guid favoriteProfileId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.FavoriteProfileIds.Contains(favoriteProfileId)) return null;

            user.FavoriteProfileIds.Add(favoriteProfileId);
            await _userRepository.UpdateAsync(user);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> RemoveAsync(Guid userId, Guid favoriteProfileId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.FavoriteProfileIds.Contains(favoriteProfileId)) return null;

            user.FavoriteProfileIds.Remove(favoriteProfileId);
            await _userRepository.UpdateAsync(user);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetFavoritesAsync(Guid userId, JsonDocument queryParams)
    {
        try
        {
            var root = queryParams.RootElement;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var allFavoriteIds = user.FavoriteProfileIds;
            var favoriteIds = allFavoriteIds.Skip((page - 1) * limit).Take(limit).ToList();
            var favorites = await _profileRepository.GetProfilesByIdsAsync(favoriteIds);

            var result = new { favorites };
            return JsonDocument.Parse(JsonSerializer.Serialize(result, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid favoriteUserId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.FavoriteProfileIds.Contains(favoriteUserId) == true;
        }
        catch
        {
            return false;
        }
    }
}