using MediatR;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для загрузки изображения мероприятия.
    /// </summary>
    public class UploadEventImageCommand : IRequest<string>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Поток с изображением.
        /// </summary>
        public Stream FileStream { get; set; } = null!;

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
    }
}