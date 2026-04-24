using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Команда для обновления профиля музыканта.
    /// </summary>
    public class UpdateProfileCommand : ICommand<Unit>, IBaseCommand
    {
        /// <summary>
        /// Полное имя / название.
        /// </summary>
        public ProfileName FullName { get; set; } = null!;

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
        /// Идентификатор города.
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// Опыт в годах.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Кого ищет.
        /// </summary>
        public LookingFor LookingFor { get; set; }

        /// <summary>
        /// Новый список жанров.
        /// </summary>
        public List<GenreId> GenreIds { get; set; } = new();

        /// <summary>
        /// Новый список специальностей.
        /// </summary>
        public List<SpecialtyId> SpecialtyIds { get; set; } = new();

        /// <summary>
        /// Новый список целей сотрудничества.
        /// </summary>
        public List<CollaborationGoalId> CollaborationGoalIds { get; set; } = new();

        /// <summary>
        /// Новые искомые жанры.
        /// </summary>
        public List<GenreId> DesiredGenreIds { get; set; } = new();

        /// <summary>
        /// Новые искомые специальности.
        /// </summary>
        public List<SpecialtyId> DesiredSpecialtyIds { get; set; } = new();

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}