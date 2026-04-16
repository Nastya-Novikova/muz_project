using backend.Models.Classes;
using backend.Models.Common;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class EventValidationService : IEventValidationService
    {
        public Result ValidateEventDates(DateTime startDateTime, DateTime? endDateTime)
        {
            if (startDateTime < DateTime.UtcNow)
                return Result.Failure("Дата начала не может быть в прошлом");

            if (endDateTime.HasValue && endDateTime.Value < startDateTime)
                return Result.Failure("Дата окончания не может быть раньше даты начала");

            return Result.Success();
        }

        public Result ValidateEventIsScheduled(Event eventEntity)
        {
            if (eventEntity.Status != Models.Enums.EventStatus.Scheduled)
                return Result.Failure("Мероприятие должно быть запланировано");

            return Result.Success();
        }

        public Result ValidateEventOwnership(Event eventEntity, Guid userId)
        {
            if (eventEntity.CreatorProfileId != userId)
                return Result.Failure("Только создатель может выполнять это действие");

            return Result.Success();
        }

        public Result ValidateEventCapacity(Event eventEntity, int currentParticipantsCount)
        {
            if (eventEntity.MaxParticipants > 0 && currentParticipantsCount >= eventEntity.MaxParticipants)
                return Result.Failure("Достигнут лимит участников");

            return Result.Success();
        }

        public Result ValidateEventNotStarted(Event eventEntity)
        {
            if (eventEntity.StartDateTime < DateTime.UtcNow)
                return Result.Failure("Мероприятие уже началось");

            return Result.Success();
        }
    }
}
