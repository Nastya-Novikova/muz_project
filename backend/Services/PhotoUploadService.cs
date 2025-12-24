using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class PhotoUploadService : IPhotoUploadService
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

            if (file.Length > 5 * 1024 * 1024) // 5 МБ
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
    }
}
