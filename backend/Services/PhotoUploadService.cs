using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Models.Classes;
using AutoMapper;
using backend.Models.Common;
using backend.Models.DTOs.Uploads;

namespace backend.Services
{
    public class PhotoUploadService : IPhotoUploadService
    {
        private readonly IPortfolioPhotoRepository _photoRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEntityExistenceService _existenceService;

        public PhotoUploadService(
            IPortfolioPhotoRepository photoRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEntityExistenceService entityExistenceService)
        {
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _existenceService = entityExistenceService;
        }

        public async Task<Result<UploadResultDto>> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description)
        {
            if (!contentType.StartsWith("image/"))
                return Result<UploadResultDto>.Failure("Only image files are allowed");

            if (fileStream.Length > 500 * 1024 * 1024)
                return Result<UploadResultDto>.Failure("File too large (max 500 MB)");

            var userResult = await _existenceService.GetUserWithProfileAsync(userId);
            if (!userResult.IsSuccess)
                return Result<UploadResultDto>.Failure(userResult.Error);
            var user = userResult.Value;

            var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

            var photo = new PortfolioPhoto
            {
                Id = Guid.NewGuid(),
                ProfileId = user.MusicianProfile.Id,
                Title = title,
                Description = description,
                FileUrl = fileUrl,
                MimeType = contentType,
                CreatedAt = DateTime.UtcNow
            };

            await _photoRepository.AddAsync(photo);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<UploadResultDto>(photo);
            dto.FileUrl = fileUrl;

            return Result<UploadResultDto>.Success(dto);
        }
    }
}
