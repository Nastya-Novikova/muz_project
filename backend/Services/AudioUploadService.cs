using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Uploads;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
namespace backend.Services;

/// <summary>
/// Сервис загрузки аудиофайлов
/// </summary>
public class AudioUploadService : IAudioUploadService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPortfolioAudioRepository _audioRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AudioUploadService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IPortfolioAudioRepository audioRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _audioRepository = audioRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UploadResultDto>> UploadAudioAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description)
    {
        // Проверка формата
        if (!contentType.StartsWith("audio/"))
            return Result<UploadResultDto>.Failure("Only audio files are allowed");

        // Проверка размера (например, 10 МБ)
        if (fileStream.Length > 10 * 1024 * 1024)
            return Result<UploadResultDto>.Failure("File too large (max 10 MB)");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<UploadResultDto>.Failure("Profile not found");

        var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
        if (profile == null)
            return Result<UploadResultDto>.Failure("Profile not found");

        // Сохраняем файл
        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        var audio = new PortfolioAudio
        {
            Id = Guid.NewGuid(),
            ProfileId = profile.Id,
            Title = title,
            Description = description,
            FileData = Array.Empty<byte>(), // после миграции заменим на FileUrl
            MimeType = contentType,
            Duration = 0, // TODO: получить длительность
            CreatedAt = DateTime.UtcNow
        };

        await _audioRepository.AddAsync(audio);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<UploadResultDto>(audio);
        dto.FileUrl = fileUrl; // временно

        return Result<UploadResultDto>.Success(dto);
    }
}

/*public class AudioUploadService : IAudioUploadService
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
}*/