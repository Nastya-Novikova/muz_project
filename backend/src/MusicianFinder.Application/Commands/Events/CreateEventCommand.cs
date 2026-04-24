using MediatR;
using MusicianFinder.Application.Commands.Base;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.ValueObjects;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для создания мероприятия.
    /// </summary>
    public class CreateEventCommand : ICommand<Guid>, IBaseCommand
    {
        /// <summary>
        /// Название.
        /// </summary>
        public EventTitle Title { get; set; } = null!;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Идентификатор региона.
        /// </summary>
        public Guid RegionId { get; set; }

        /// <summary>
        /// Идентификатор города.
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// Адрес.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время начала.
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Дата и время окончания (необязательно).
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Максимальное количество участников (0 — без ограничений).
        /// </summary>
        public int MaxParticipants { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}