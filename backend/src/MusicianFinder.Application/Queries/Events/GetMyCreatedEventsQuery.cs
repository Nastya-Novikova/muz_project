using MediatR;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Запрос для получения мероприятий, созданных текущим пользователем.
    /// </summary>
    public class GetMyCreatedEventsQuery : IQuery<PagedResult<EventDto>>
    {
        /// <summary>Номер страницы.</summary>
        public int Page { get; set; } = 1;
        /// <summary>Размер страницы.</summary>
        public int Limit { get; set; } = 20;
    }
}