using MediatR;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Команда для загрузки медиафайла в портфолио текущего пользователя.
    /// </summary>
    public class UploadMediaCommand : IRequest<Guid>, IBaseCommand
    {
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

        /// <summary>
        /// Название медиа.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Тип медиа (аудио, видео, фото).
        /// </summary>
        public MediaType Type { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}