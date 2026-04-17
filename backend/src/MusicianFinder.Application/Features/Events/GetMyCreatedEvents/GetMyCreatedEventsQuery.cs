using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Pagination;
using MusicianFinder.Application.Features.Events.DTOs;

namespace MusicianFinder.Application.Features.Events.GetMyCreatedEvents
{
    /// <summary>
    /// Запрос для получения мероприятий, созданных текущим пользователем.
    /// </summary>
    public class GetMyCreatedEventsQuery : IRequest<PagedResult<EventDto>>
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