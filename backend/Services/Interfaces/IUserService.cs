using backend.Models.Common;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.Profiles;
using backend.Models.DTOs.User;
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
    Task<Result<UserDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Обновить профиль пользователя
    /// </summary>
    Task<Result<UserDto>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request);

    /// <summary>
    /// Удалить пользователя (soft-delete)
    /// </summary>
    Task<Result> DeleteAsync(Guid userId);
}