using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Application.Features.Metadata.DTOs;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Features.Events.DTOs
{
    /// <summary>
    /// DTO мероприятия.
    /// </summary>
    public class EventDto
    {
        /// <summary>
        /// Идентификатор мероприятия.
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
        /// URL изображения.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Регион.
        /// </summary>
        public LookupItemDto Region { get; set; } = new();

        /// <summary>
        /// Город.
        /// </summary>
        public LookupItemDto City { get; set; } = new();

        /// <summary>
        /// Адрес.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время начала.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Дата и время окончания.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Максимальное количество участников.
        /// </summary>
        public int MaxParticipants { get; set; }

        /// <summary>
        /// Текущее количество участников.
        /// </summary>
        public int CurrentParticipants { get; set; }

        /// <summary>
        /// Зарегистрирован ли текущий пользователь.
        /// </summary>
        public bool IsRegistered { get; set; }

        /// <summary>
        /// Является ли пользователь создателем мероприятия.
        /// </summary>
        public bool IsCreator { get; set; }

        /// <summary>
        /// Статус мероприятия.
        /// </summary>
        public EventStatus Status { get; set; }

        /// <summary>
        /// Идентификатор профиля создателя.
        /// </summary>
        public Guid CreatorProfileId { get; set; }

        /// <summary>
        /// Полное имя создателя.
        /// </summary>
        public string CreatorFullName { get; set; } = string.Empty;

        /// <summary>
        /// URL аватара создателя.
        /// </summary>
        public string? CreatorAvatarUrl { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата обновления.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}