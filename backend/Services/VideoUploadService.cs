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
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPortfolioVideoRepository _videoRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VideoUploadService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IPortfolioVideoRepository videoRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _videoRepository = videoRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UploadResultDto>> UploadVideoAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description)
    {
        if (!contentType.StartsWith("video/"))
            return Result<UploadResultDto>.Failure("Only video files are allowed");

        if (fileStream.Length > 500 * 1024 * 1024)
            return Result<UploadResultDto>.Failure("File too large (max 500 MB)");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<UploadResultDto>.Failure("Profile not found");

        var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
        if (profile == null)
            return Result<UploadResultDto>.Failure("Profile not found");

        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        var video = new PortfolioVideo
        {
            Id = Guid.NewGuid(),
            ProfileId = profile.Id,
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