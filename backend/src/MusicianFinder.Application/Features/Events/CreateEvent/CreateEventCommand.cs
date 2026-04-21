using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Events.CreateEvent
{
    /// <summary>
    /// Команда для создания нового мероприятия.
    /// </summary>
    public class CreateEventCommand : IRequest<Guid>
    {
        /// <summary>
        /// Название мероприятия.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание мероприятия.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Идентификатор региона.
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// Идентификатор города.
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Адрес проведения.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время начала.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Дата и время окончания (может быть не указано).
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Максимальное количество участников (0 — без ограничений).
        /// </summary>
        public int MaxParticipants { get; set; }
    }
}