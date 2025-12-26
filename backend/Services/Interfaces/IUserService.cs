using System.Text.Json;

namespace backend.Services.Interfaces;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    Task<JsonDocument?> GetByIdAsync(Guid id);

    /// <summary>
    /// Обновить профиль пользователя
    /// </summary>
    Task<JsonDocument?> UpdateProfileAsync(Guid userId, JsonDocument profileJson);

    /// <summary>
    /// Удалить пользователя (soft-delete)
    /// </summary>
    Task<JsonDocument?> DeleteAsync(Guid userId);
}