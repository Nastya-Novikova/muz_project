using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Events.CancelEvent
{
    /// <summary>
    /// Команда для отмены мероприятия.
    /// </summary>
    public class CancelEventCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}