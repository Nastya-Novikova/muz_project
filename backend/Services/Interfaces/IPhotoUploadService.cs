namespace backend.Services.Interfaces
{
    public interface IPhotoUploadService
    {
        Task<object> UploadPhotoAsync(Guid profileId, IFormFile file, string title, string? description = null);
    }
}
