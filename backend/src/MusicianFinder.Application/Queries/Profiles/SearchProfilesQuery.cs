using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для поиска профилей с фильтрацией и пагинацией.
    /// </summary>
    public class SearchProfilesQuery : IRequest<PagedResult<ProfileDto>>
    {
        /// <summary>
        /// Поисковый запрос.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Идентификатор города.
        /// </summary>
        public int? CityId { get; set; }

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
        public List<int>? GoalIds { get; set; }

        /// <summary>
        /// Минимальный опыт.
        /// </summary>
        public int? ExperienceMin { get; set; }

        /// <summary>
        /// Максимальный опыт.
        /// </summary>
        public int? ExperienceMax { get; set; }

        /// <summary>
        /// Кого ищет.
        /// </summary>
        public LookingFor? LookingFor { get; set; }

        /// <summary>
        /// Тип профиля.
        /// </summary>
        public ProfileType? ProfileType { get; set; }

        /// <summary>
        /// Идентификаторы искомых жанров.
        /// </summary>
        public List<int>? DesiredGenreIds { get; set; }

        /// <summary>
        /// Идентификаторы искомых специальностей.
        /// </summary>
        public List<int>? DesiredSpecialtyIds { get; set; }

        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Поле сортировки.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки.
        /// </summary>
        public bool SortDesc { get; set; } = true;
    }
}