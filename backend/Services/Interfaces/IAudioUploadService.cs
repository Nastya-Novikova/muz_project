namespace backend.Services.Interfaces;

/// <summary>
/// Сервис загрузки аудиофайлов
/// </summary>
public interface IAudioUploadService
{
    /// <summary>
    /// Загрузить аудиофайл в портфолио
    /// </summary>
    Task<object> UploadAudioAsync(
        Guid profileId,
        IFormFile file,
        string title,
        string? description = null);
}