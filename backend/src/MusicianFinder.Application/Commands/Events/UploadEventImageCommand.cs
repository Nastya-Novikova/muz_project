using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для загрузки изображения мероприятия.
    /// </summary>
    public class UploadEventImageCommand : IRequest<string>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Содержимое файла.
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}