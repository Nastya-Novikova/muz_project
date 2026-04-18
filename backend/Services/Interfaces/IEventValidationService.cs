using backend.Models.Classes;
using backend.Models.Common;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Сервис для бизнес-валидации мероприятий.
    /// </summary>
    public interface IEventValidationService
    {
        /// <summary>
        /// Проверяет, что дата начала в будущем, а дата окончания (если указана) позже даты начала.
        /// </summary>
        Result ValidateEventDates(DateTime startDateTime, DateTime? endDateTime);

        /// <summary>
        /// Проверяет, что мероприятие находится в статусе Scheduled.
        /// </summary>
        Result ValidateEventIsScheduled(Event eventEntity);

        /// <summary>
        /// Проверяет, что пользователь является создателем мероприятия.
        /// </summary>
        Result ValidateEventOwnership(Event eventEntity, Guid userId);

        /// <summary>
        /// Проверяет, что на мероприятии ещё есть свободные места.
        /// </summary>
        Result ValidateEventCapacity(Event eventEntity, int currentParticipantsCount);

        /// <summary>
        /// Проверяет, что мероприятие ещё не началось.
        /// </summary>
        Result ValidateEventNotStarted(Event eventEntity);
    }
}
