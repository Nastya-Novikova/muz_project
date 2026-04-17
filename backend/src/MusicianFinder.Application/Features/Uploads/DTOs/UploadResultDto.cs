using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Application.Features.Uploads.DTOs
{
    /// <summary>
    /// DTO результата загрузки файла.
    /// </summary>
    public class UploadResultDto
    {
        /// <summary>
        /// Идентификатор созданной записи.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL файла.
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип.
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// Длительность (для аудио/видео).
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}