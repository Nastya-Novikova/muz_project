using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Сервис загрузки видеофайлов
/// </summary>
public class VideoUploadService : IVideoUploadService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IPortfolioVideoRepository _videoRepository;

    public VideoUploadService(
        IProfileRepository profileRepository,
        IPortfolioVideoRepository videoRepository)
    {
        _profileRepository = profileRepository;
        _videoRepository = videoRepository;
    }

    public async Task<object> UploadVideoAsync(
        Guid profileId,
        IFormFile file,
        string title,
        string? description = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран");

        if (!IsVideo(file.ContentType))
            throw new ArgumentException("Разрешены только видеофайлы");

        if (file.Length > 50 * 1024 * 1024)
            throw new ArgumentException("Файл слишком большой");

        var profile = await _profileRepository.GetByIdAsync(profileId);
        if (profile == null)
            throw new ArgumentException("Профиль не найден");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var video = new PortfolioVideo
        {
            Id = Guid.NewGuid(),
            ProfileId = profileId,
            Title = title,
            Description = description,
            FileData = fileBytes,
            MimeType = file.ContentType,
            Duration = 0 //TODO:Получить продолжительность
        };

        await _videoRepository.AddAsync(video);

        return new
        {
            success = true,
            video = new
            {
                video.Id,
                video.Title,
                video.Description,
                video.MimeType,
                video.Duration,
                video.CreatedAt
            }
        };
    }

    private static bool IsVideo(string contentType) =>
        contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
}