using MediatR;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Команда для создания профиля музыканта.
    /// </summary>
    public class CreateProfileCommand : IRequest<Guid>
    {
        /// <summary>
        /// Тип профиля.
        /// </summary>
        public ProfileType ProfileType { get; set; }

        /// <summary>
        /// Полное имя / название.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

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
        /// Идентификаторы жанров.
        /// </summary>
        public List<int>? GenreIds { get; set; }

        /// <summary>
        /// Идентификаторы специальностей.
        /// </summary>
        public List<int>? SpecialtyIds { get; set; }

        /// <summary>
        /// Идентификаторы целей сотрудничества.
        /// </summary>
        public List<int>? CollaborationGoalIds { get; set; }

        /// <summary>
        /// Идентификаторы искомых жанров.
        /// </summary>
        public List<int>? DesiredGenreIds { get; set; }

        /// <summary>
        /// Идентификаторы искомых специальностей.
        /// </summary>
        public List<int>? DesiredSpecialtyIds { get; set; }
    }
}