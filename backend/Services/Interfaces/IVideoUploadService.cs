using backend.Models.Common;
using backend.Models.DTOs.Uploads;

namespace backend.Services.Interfaces;

/// <summary>
/// Сервис загрузки видеофайлов
/// </summary>
public interface IVideoUploadService
{
    /// <summary>
    /// Загрузить видеофайл в портфолио
    /// </summary>
    Task<Result<UploadResultDto>> UploadVideoAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description);
}