using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Uploads;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services;

/// <summary>
/// Сервис загрузки видеофайлов
/// </summary>
public class VideoUploadService : IVideoUploadService
{
    private readonly IPortfolioVideoRepository _videoRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEntityExistenceService _existenceService;

    public VideoUploadService(
        IPortfolioVideoRepository videoRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEntityExistenceService entityExistenceService)
    {
        _videoRepository = videoRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _existenceService = entityExistenceService;
    }

    public async Task<Result<UploadResultDto>> UploadVideoAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description)
    {
        if (!contentType.StartsWith("video/"))
            return Result<UploadResultDto>.Failure("Only video files are allowed");

        if (fileStream.Length > 500 * 1024 * 1024)
            return Result<UploadResultDto>.Failure("File too large (max 500 MB)");

        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<UploadResultDto>.Failure(userResult.Error);
        var user = userResult.Value;

        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        var video = new PortfolioVideo
        {
            Id = Guid.NewGuid(),
            ProfileId = user.MusicianProfile.Id,
            Title = title,
            Description = description,
            FileUrl = fileUrl,
            MimeType = contentType,
            Duration = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _videoRepository.AddAsync(video);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<UploadResultDto>(video);
        dto.FileUrl = fileUrl;

        return Result<UploadResultDto>.Success(dto);
    }
}