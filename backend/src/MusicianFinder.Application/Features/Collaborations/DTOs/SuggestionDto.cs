using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Application.Features.Profiles.DTOs;

namespace MusicianFinder.Application.Features.Collaborations.DTOs
{
    /// <summary>
    /// DTO предложения о сотрудничестве.
    /// </summary>
    public class SuggestionDto
    {
        /// <summary>
        /// Идентификатор предложения.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Профиль отправителя.
        /// </summary>
        public ProfileDto FromProfile { get; set; } = new();

        /// <summary>
        /// Профиль получателя.
        /// </summary>
        public ProfileDto ToProfile { get; set; } = new();

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Статус.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}