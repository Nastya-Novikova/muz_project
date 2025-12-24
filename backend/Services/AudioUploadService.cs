using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Сервис загрузки аудиофайлов
/// </summary>
public class AudioUploadService : IAudioUploadService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IPortfolioAudioRepository _audioRepository;

    public AudioUploadService(
        IProfileRepository profileRepository,
        IPortfolioAudioRepository audioRepository)
    {
        _profileRepository = profileRepository;
        _audioRepository = audioRepository;
    }

    public async Task<object> UploadAudioAsync(
        Guid profileId,
        IFormFile file,
        string title,
        string? description = null)
    {
        // Валидация
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран");

        if (!IsAudio(file.ContentType))
            throw new ArgumentException("Разрешены только аудиофайлы");

        if (file.Length > 10 * 1024 * 1024) // 10 МБ
            throw new ArgumentException("Файл слишком большой");

        // Проверка существования профиля
        var profile = await _profileRepository.GetByIdAsync(profileId);
        if (profile == null)
            throw new ArgumentException("Профиль не найден");

        // Чтение файла
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        // Создание записи
        var audio = new PortfolioAudio
        {
            Id = Guid.NewGuid(),
            ProfileId = profileId,
            Title = title,
            Description = description,
            FileData = fileBytes,
            MimeType = file.ContentType,
            Duration = 0
        };

        await _audioRepository.AddAsync(audio);

        return new
        {
            success = true,
            audio = new
            {
                audio.Id,
                audio.Title,
                audio.Description,
                audio.MimeType,
                audio.Duration,
                audio.CreatedAt
            }
        };
    }

    private static bool IsAudio(string contentType) =>
        contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
}