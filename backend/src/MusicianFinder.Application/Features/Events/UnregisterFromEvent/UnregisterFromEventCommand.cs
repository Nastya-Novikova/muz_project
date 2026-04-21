using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Events.UnregisterFromEvent
{
    /// <summary>
    /// Команда для отмены регистрации с мероприятия.
    /// </summary>
    public class UnregisterFromEventCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}