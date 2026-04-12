using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Profiles;
using System.Text.Json;

namespace backend.Services.Interfaces;

public interface IProfileService
{
    Task<Result<PagedResult<ProfileDto>>> SearchAsync(SearchRequest request);
    Task<Result<ProfileDto>> GetByIdAsync(Guid id);
    Task<Result<ProfileDto>> GetByUserIdAsync(Guid userId);
    Task<Result<ProfileDto>> CreateAsync(Guid userId, CreateProfileRequest request);
    Task<Result<ProfileDto>> UpdateAsync(Guid userId, UpdateProfileRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<string>> UpdateAvatarAsync(Guid userId, Stream fileStream, string fileName, string contentType);
    Task<Result<object>> GetMediaAsync(Guid id);
    Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(Guid userId);
}