using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Видеозапись в портфолио.
    /// </summary>
    public class PortfolioVideo
    {
        private PortfolioVideo() { }

        public PortfolioVideo(Guid profileId, string title, string fileUrl, string mimeType, int duration = 0, string? description = null)
        {
            Id = Guid.NewGuid();
            ProfileId = profileId;
            Title = title;
            Description = description;
            FileUrl = fileUrl;
            MimeType = mimeType;
            Duration = duration;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// ID профиля владельца.
        /// </summary>
        public Guid ProfileId { get; private set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// URL видеофайла.
        /// </summary>
        public string FileUrl { get; private set; }

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string MimeType { get; private set; }

        /// <summary>
        /// Продолжительность в секундах.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        // Навигационное свойство
        public MusicianProfile? Profile { get; private set; }
    }
}
