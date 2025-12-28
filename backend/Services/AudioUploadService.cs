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
    private readonly IUserRepository _userRepository;

    public AudioUploadService(
        IProfileRepository profileRepository,
        IPortfolioAudioRepository audioRepository, IUserRepository userRepository)
    {
        _profileRepository = profileRepository;
        _audioRepository = audioRepository;
        _userRepository = userRepository;
    }

    public async Task<object> UploadAudioAsync(
        Guid userId,
        IFormFile file,
        string title,
        string? description = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.MusicianProfile == null) return null;
        var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
        if (profile == null) return null;


        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран");

        if (!IsAudio(file.ContentType))
            throw new ArgumentException("Разрешены только аудиофайлы");

        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("Файл слишком большой");

        if (profile == null)
            throw new ArgumentException("Профиль не найден");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var audio = new PortfolioAudio
        {
            Id = Guid.NewGuid(),
            ProfileId = profile.Id,
            Title = title,
            Description = description,
            FileData = fileBytes,
            MimeType = file.ContentType,
            Duration = 0 //TODO:Получить продолжительность
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