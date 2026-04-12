namespace backend.Services.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteFileAsync(string fileUrl);
        //string GetFileUrl(string fileName);
    }
}
