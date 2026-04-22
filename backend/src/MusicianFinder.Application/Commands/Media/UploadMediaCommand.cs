using MediatR;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Media
{
    /// <summary>
    /// Команда для загрузки медиафайла в портфолио текущего пользователя.
    /// </summary>
    public class UploadMediaCommand : IRequest<Guid>
    {
        /// <summary>
        /// Поток с файлом.
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
    }
}