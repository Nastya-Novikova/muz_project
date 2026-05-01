using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Команда для создания профиля музыканта.
    /// </summary>
    public class CreateProfileCommand : ICommand<Guid>, IBaseCommand
    {
        /// <summary>
        /// Вид профиля.
        /// </summary>
        public ProfileType ProfileType { get; set; }

        /// <summary>
        /// Полное имя / название.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Возраст (опционально).
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
        public int CityId { get; set; }

        /// <summary>
        /// Опыт в годах.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// Кого ищет.
        /// </summary>
        public LookingFor LookingFor { get; set; }

        /// <summary>
        /// Идентификаторы предлагаемых жанров.
        /// </summary>
        public List<int> GenreIds { get; set; } = new();

        /// <summary>
        /// Идентификаторы специальностей.
        /// </summary>
        public List<int> SpecialtyIds { get; set; } = new();

        /// <summary>
        /// Идентификаторы целей сотрудничества.
        /// </summary>
        public List<int> CollaborationGoalIds { get; set; } = new();

        /// <summary>
        /// Идентификаторы искомых жанров.
        /// </summary>
        public List<int> DesiredGenreIds { get; set; } = new();

        /// <summary>
        /// Идентификаторы искомых специальностей.
        /// </summary>
        public List<int> DesiredSpecialtyIds { get; set; } = new();

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}