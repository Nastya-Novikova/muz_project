namespace backend.Services.Interfaces;

/// <summary>
/// Сервис загрузки видеофайлов
/// </summary>
public interface IVideoUploadService
{
    /// <summary>
    /// Загрузить видеофайл в портфолио
    /// </summary>
    Task<object> UploadVideoAsync(
        Guid profileId,
        IFormFile file,
        string title,
        string? description = null);
}