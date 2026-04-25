using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.DTOs.Metadata;

namespace MusicianFinder.Application.DTOs.Profiles
{
    /// <summary>
    /// Полный DTO профиля музыканта.
    /// </summary>
    public class ProfileDto
    {
        /// <summary>Идентификатор профиля.</summary>
        public Guid Id { get; set; }

        /// <summary>Полное имя / название.</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>URL аватара.</summary>
        public string? AvatarUrl { get; set; }

        /// <summary>Возраст.</summary>
        public int? Age { get; set; }

        /// <summary>Описание.</summary>
        public string? Description { get; set; }

        /// <summary>Телефон.</summary>
        public string? Phone { get; set; }

        /// <summary>Имя пользователя Telegram.</summary>
        public string? Telegram { get; set; }

        /// <summary>Город.</summary>
        public LookupItemDto City { get; set; } = new();

        /// <summary>Опыт в годах.</summary>
        public int Experience { get; set; }

        /// <summary>Кого ищет.</summary>
        public string LookingFor { get; set; } = string.Empty;

        /// <summary>Email.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Жанры.</summary>
        public List<LookupItemDto> Genres { get; set; } = new();

        /// <summary>Специальности.</summary>
        public List<LookupItemDto> Specialties { get; set; } = new();

        /// <summary>Цели сотрудничества.</summary>
        public List<LookupItemDto> CollaborationGoals { get; set; } = new();

        /// <summary>Искомые жанры.</summary>
        public List<LookupItemDto> DesiredGenres { get; set; } = new();

        /// <summary>Искомые специальности.</summary>
        public List<LookupItemDto> DesiredSpecialties { get; set; } = new();

        /// <summary>Является ли профилем текущего пользователя.</summary>
        public bool IsMyProfile { get; set; }

        /// <summary>Добавлен ли в избранное текущим пользователем.</summary>
        public bool IsFavorite { get; set; }

        /// <summary>Отправлено ли текущим пользователем предложение о сотрудничестве.</summary>
        public bool IsCollaborated { get; set; }

        public List<AudioDto> Audio { get; set; } = new();
        public List<VideoDto> Video { get; set; } = new();
        public List<PhotoDto> Photos { get; set; } = new();
    }
}