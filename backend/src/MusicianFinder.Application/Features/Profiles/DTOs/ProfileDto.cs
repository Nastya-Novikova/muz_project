using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Application.Features.Metadata.DTOs;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Features.Profiles.DTOs
{
    /// <summary>
    /// DTO профиля музыканта.
    /// </summary>
    public class ProfileDto
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип профиля.
        /// </summary>
        public ProfileType ProfileType { get; set; }

        /// <summary>
        /// Полное имя.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// URL аватара.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Возраст.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Telegram.
        /// </summary>
        public string? Telegram { get; set; }

        /// <summary>
        /// Город.
        /// </summary>
        public LookupItemDto City { get; set; } = new();

        /// <summary>
        /// Опыт в годах.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Кого ищет.
        /// </summary>
        public LookingFor LookingFor { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Уведомления по email.
        /// </summary>
        public bool NotifyByEmail { get; set; }

        /// <summary>
        /// Уведомления по VK.
        /// </summary>
        public bool NotifyByVk { get; set; }

        /// <summary>
        /// Жанры.
        /// </summary>
        public List<LookupItemDto> Genres { get; set; } = new();

        /// <summary>
        /// Специальности.
        /// </summary>
        public List<LookupItemDto> Specialties { get; set; } = new();

        /// <summary>
        /// Цели сотрудничества.
        /// </summary>
        public List<LookupItemDto> CollaborationGoals { get; set; } = new();

        /// <summary>
        /// Искомые жанры.
        /// </summary>
        public List<LookupItemDto> DesiredGenres { get; set; } = new();

        /// <summary>
        /// Искомые специальности.
        /// </summary>
        public List<LookupItemDto> DesiredSpecialties { get; set; } = new();
    }
}