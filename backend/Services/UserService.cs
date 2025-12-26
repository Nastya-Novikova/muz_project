using System.Text.Json;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<JsonDocument?> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : JsonDocument.Parse(JsonSerializer.Serialize(user, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> UpdateProfileAsync(Guid userId, JsonDocument profileJson)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var root = profileJson.RootElement;

            if (root.TryGetProperty("favoriteProfileIds", out var favIds))
            {
                user.FavoriteProfileIds = favIds.EnumerateArray().Select(x => Guid.Parse(x.GetString()!)).ToList();
            }

            await _userRepository.UpdateAsync(user);
            return JsonDocument.Parse(JsonSerializer.Serialize(user, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> DeleteAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            await _userRepository.SoftDeleteAsync(userId);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }
}