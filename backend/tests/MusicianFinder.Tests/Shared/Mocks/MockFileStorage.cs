using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Tests.Shared.Mocks
{
    public class MockFileStorage : IFileStorage
    {
        public string? LastSavedUrl { get; private set; }
        public string? LastDeletedUrl { get; private set; }

        /// <summary>
        /// Если true, метод SaveFileAsync будет выбрасывать исключение.
        /// </summary>
        public bool ShouldThrowOnSave { get; set; }

        public Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
        {
            if (ShouldThrowOnSave)
                throw new Exception("MinIO unavailable");

            var url = $"http://mock.storage/{Guid.NewGuid()}/{fileName}";
            LastSavedUrl = url;
            return Task.FromResult(url);
        }

        public Task DeleteFileAsync(string fileUrl)
        {
            LastDeletedUrl = fileUrl;
            return Task.CompletedTask;
        }
    }
}