using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Запрос для получения списка мероприятий с фильтрацией и пагинацией.
    /// </summary>
    public class GetEventsQuery : IRequest<PagedResult<EventDto>>
    {
        /// <summary>
        /// Поисковый запрос по названию и описанию.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Фильтр по региону.
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// Фильтр по городу.
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Фильтр по дате начала (с).
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Фильтр по дате начала (по).
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Фильтр по статусу мероприятия.
        /// </summary>
        public EventStatus? Status { get; set; }

        /// <summary>
        /// Фильтр по создателю.
        /// </summary>
        public Guid? CreatorProfileId { get; set; }

        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Поле для сортировки (title, startdatetime, createdat).
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки (true — по убыванию).
        /// </summary>
        public bool SortDesc { get; set; } = true;
    }
}