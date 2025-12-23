using System.Text.Json;

namespace backend.Services.Interfaces;

public interface IFavoriteService
{
    Task<JsonDocument?> AddAsync(Guid userId, Guid favoriteUserId);
    Task<JsonDocument?> RemoveAsync(Guid userId, Guid favoriteUserId);
    Task<JsonDocument?> GetFavoritesAsync(Guid userId, JsonDocument queryParams);
}