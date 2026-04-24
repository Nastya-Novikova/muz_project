using MediatR;
using MusicianFinder.Application.Core.Behaviors;

namespace MusicianFinder.Application.Commands.Events
{
    /// <summary>
    /// Команда для обновления мероприятия.
    /// </summary>
    public class UpdateEventCommand : IRequest<Unit>, IBaseCommand
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Новое название.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Новое описание.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Новый идентификатор региона.
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// Новый идентификатор города.
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Новый адрес.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Новая дата начала.
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Новая дата окончания.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Новое максимальное количество участников.
        /// </summary>
        public int? MaxParticipants { get; set; }

        /// <inheritdoc />
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}