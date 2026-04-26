namespace MusicianFinder.API.Contracts.Responses
{
    /// <summary>
    /// Универсальный ответ при загрузке файла.
    /// </summary>
    public class FileUploadResultDto
    {
        /// <summary>
        /// URL загруженного файла.
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}