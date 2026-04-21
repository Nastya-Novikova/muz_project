using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Features.Events.DTOs;

namespace MusicianFinder.Application.Features.Events.GetEventById
{
    /// <summary>
    /// Запрос для получения мероприятия по идентификатору.
    /// </summary>
    public class GetEventByIdQuery : IRequest<EventDto>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}