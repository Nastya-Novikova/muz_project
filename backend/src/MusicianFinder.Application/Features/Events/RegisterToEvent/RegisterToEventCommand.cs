using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Events.RegisterToEvent
{
    /// <summary>
    /// Команда для регистрации на мероприятие.
    /// </summary>
    public class RegisterToEventCommand : IRequest
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }
    }
}