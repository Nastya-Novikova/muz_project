namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для работы с файловым хранилищем (например, MinIO).
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Сохраняет файл в хранилище и возвращает публичный URL.
        /// </summary>
        /// <param name="fileStream">Поток с содержимым файла.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="contentType">MIME-тип файла.</param>
        /// <returns>URL сохранённого файла.</returns>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Удаляет файл из хранилища по его URL.
        /// </summary>
        /// <param name="fileUrl">URL файла.</param>
        Task DeleteFileAsync(string fileUrl);
    }
}