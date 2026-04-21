using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Features.Profiles.UpdateProfile
{
    /// <summary>
    /// Команда для обновления профиля музыканта.
    /// </summary>
    public class UpdateProfileCommand : IRequest<Unit>
    {
        /// <summary>
        /// Тип профиля.
        /// </summary>
        public ProfileType? ProfileType { get; set; }

        /// <summary>
        /// Полное имя.
        /// </summary>
        public string? FullName { get; set; }

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
        public int? CityId { get; set; }

        /// <summary>
        /// Опыт.
        /// </summary>
        public int? Experience { get; set; }

        /// <summary>
        /// Кого ищет.
        /// </summary>
        public LookingFor? LookingFor { get; set; }

        /// <summary>
        /// Уведомления по email.
        /// </summary>
        public bool? NotifyByEmail { get; set; }

        /// <summary>
        /// Уведомления по VK.
        /// </summary>
        public bool? NotifyByVk { get; set; }

        /// <summary>
        /// Идентификаторы жанров.
        /// </summary>
        public List<int>? GenreIds { get; set; }

        /// <summary>
        /// Идентификаторы специальностей.
        /// </summary>
        public List<int>? SpecialtyIds { get; set; }

        /// <summary>
        /// Идентификаторы целей.
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