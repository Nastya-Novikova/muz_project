using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для поиска профилей с фильтрацией и пагинацией.
    /// </summary>
    public class SearchProfilesQuery : IQuery<PagedResult<ProfileDto>>
    {
        /// <summary>Поисковый запрос.</summary>
        public string? Query { get; set; }
        /// <summary>Идентификатор города.</summary>
        public Guid? CityId { get; set; }
        /// <summary>Идентификаторы жанров.</summary>
        public List<Guid>? GenreIds { get; set; }
        /// <summary>Идентификаторы специальностей.</summary>
        public List<Guid>? SpecialtyIds { get; set; }
        /// <summary>Идентификаторы целей.</summary>
        public List<Guid>? GoalIds { get; set; }
        /// <summary>Минимальный опыт.</summary>
        public int? ExperienceMin { get; set; }
        /// <summary>Максимальный опыт.</summary>
        public int? ExperienceMax { get; set; }
        /// <summary>Кого ищет.</summary>
        public string? LookingFor { get; set; }
        /// <summary>Номер страницы.</summary>
        public int Page { get; set; } = 1;
        /// <summary>Размер страницы.</summary>
        public int Limit { get; set; } = 20;
        /// <summary>Поле сортировки.</summary>
        public string? SortBy { get; set; }
        /// <summary>Сортировка по убыванию.</summary>
        public bool SortDesc { get; set; } = true;
    }
}