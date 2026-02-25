using backend.Models.Common;
using backend.Models.DTOs.Uploads;

namespace backend.Services.Interfaces
{
    public interface IPhotoUploadService
    {
        Task<Result<UploadResultDto>> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType, string title, string? description);
    }
}
