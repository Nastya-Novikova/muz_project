using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.DTOs.Events;

namespace MusicianFinder.Application.Queries.Events
{
    /// <summary>
    /// Запрос для получения мероприятий, на которые зарегистрирован текущий пользователь.
    /// </summary>
    public class GetMyRegisteredEventsQuery : IRequest<PagedResult<EventDto>>
    {
        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int Limit { get; set; } = 20;
    }
}