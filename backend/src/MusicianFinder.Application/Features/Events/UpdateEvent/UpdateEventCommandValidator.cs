using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Events.UpdateEvent
{
    /// <summary>
    /// Валидатор команды <see cref="UpdateEventCommand"/>.
    /// </summary>
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateEventCommandValidator"/>.
        /// </summary>
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Идентификатор мероприятия обязателен.");

            RuleFor(x => x.Title)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Название не должно превышать 200 символов.");

            RuleFor(x => x.Address)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Address))
                .WithMessage("Адрес не должен превышать 200 символов.");

            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).When(x => x.StartDateTime.HasValue)
                .WithMessage("Дата начала не может быть в прошлом.");

            RuleFor(x => x.EndDateTime)
                .GreaterThan(x => x.StartDateTime).When(x => x.EndDateTime.HasValue && x.StartDateTime.HasValue)
                .WithMessage("Дата окончания должна быть позже даты начала.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThanOrEqualTo(0).When(x => x.MaxParticipants.HasValue)
                .WithMessage("Количество участников не может быть отрицательным.");
        }
    }
}