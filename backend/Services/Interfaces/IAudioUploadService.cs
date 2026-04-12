using backend.Models.Common;
using backend.Models.DTOs.Uploads;

namespace backend.Services.Interfaces;

/// <summary>
/// Сервис загрузки аудиофайлов
/// </summary>
public interface IAudioUploadService
{
    /// <summary>
    /// Загрузить аудиофайл в портфолио
    /// </summary>
    Task<Result<UploadResultDto>> UploadAudioAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description);
}