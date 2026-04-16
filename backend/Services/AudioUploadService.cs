using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Events;
using backend.Models.DTOs.Uploads;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
namespace backend.Services;

/// <summary>
/// Сервис загрузки аудиофайлов
/// </summary>
public class AudioUploadService : IAudioUploadService
{
    private readonly IPortfolioAudioRepository _audioRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEntityExistenceService _existenceService;

    public AudioUploadService(
        IPortfolioAudioRepository audioRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEntityExistenceService existenceService)
    {
        _audioRepository = audioRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _existenceService = existenceService;
    }

    public async Task<Result<UploadResultDto>> UploadAudioAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description)
    {
        if (!contentType.StartsWith("audio/"))
            return Result<UploadResultDto>.Failure("Only audio files are allowed");

        if (fileStream.Length > 1000 * 1024 * 1024)
            return Result<UploadResultDto>.Failure("File too large (max 1000 MB)");

        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<UploadResultDto>.Failure(userResult.Error);
        var user = userResult.Value;

        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        var audio = new PortfolioAudio
        {
            Id = Guid.NewGuid(),
            ProfileId = user.MusicianProfile.Id,
            Title = title,
            Description = description,
            FileUrl = fileUrl,
            MimeType = contentType,
            Duration = 0, // TODO: получить длительность
            CreatedAt = DateTime.UtcNow
        };

        await _audioRepository.AddAsync(audio);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<UploadResultDto>(audio);
        dto.FileUrl = fileUrl;

        return Result<UploadResultDto>.Success(dto);
    }
}