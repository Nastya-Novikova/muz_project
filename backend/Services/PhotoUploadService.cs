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
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPortfolioPhotoRepository _photoRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotoUploadService(
            IProfileRepository profileRepository,
            IUserRepository userRepository,
            IPortfolioPhotoRepository photoRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UploadResultDto>> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description)
        {
            if (!contentType.StartsWith("image/"))
                return Result<UploadResultDto>.Failure("Only image files are allowed");

            if (fileStream.Length > 500 * 1024 * 1024)
                return Result<UploadResultDto>.Failure("File too large (max 500 MB)");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.MusicianProfile == null)
                return Result<UploadResultDto>.Failure("Profile not found");

            var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
            if (profile == null)
                return Result<UploadResultDto>.Failure("Profile not found");

            var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

            var photo = new PortfolioPhoto
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
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

    /*    public class PhotoUploadService : IPhotoUploadService
        {
            private readonly IProfileRepository _profileRepository;
            private readonly IPortfolioPhotoRepository _photoRepository;

            public PhotoUploadService(IProfileRepository profileRepository, IPortfolioPhotoRepository photoRepository)
            {
                _profileRepository = profileRepository;
                _photoRepository = photoRepository;
            }

            public async Task<object> UploadPhotoAsync(Guid profileId, IFormFile file, string title, string? description = null)
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Файл не выбран");

                if (!IsImage(file.ContentType))
                    throw new ArgumentException("Разрешены только изображения");

                if (file.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("Файл слишком большой");

                var profile = await _profileRepository.GetByIdAsync(profileId);
                if (profile == null)
                    throw new ArgumentException("Профиль не найден");

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var photo = new PortfolioPhoto
                {
                    Id = Guid.NewGuid(),
                    ProfileId = profileId,
                    Title = title,
                    Description = description,
                    FileData = fileBytes,
                    MimeType = file.ContentType
                };

                await _photoRepository.AddAsync(photo);

                return new
                {
                    success = true,
                    photo = new
                    {
                        photo.Id,
                        photo.Title,
                        photo.Description,
                        photo.MimeType,
                        photo.CreatedAt
                    }
                };
            }

            private static bool IsImage(string contentType) =>
                contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }*/
}
