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

    public async Task<bool> UpdateAvatarAsync(Guid userId, byte[] avatarBytes)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.Avatar = avatarBytes;
            await _userRepository.UpdateAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<JsonDocument?> UpdateProfileAsync(Guid userId, JsonDocument profileJson)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var root = profileJson.RootElement;
            user.FullName = root.TryGetProperty("fullName", out var fn) ? fn.GetString() ?? user.FullName : user.FullName;
            user.ProfileCompleted = true; // После первого обновления профиль считается завершённым

            // Обновление избранных профилей (если передано)
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