using FluentValidation;
using MusicianFinder.Application.Commands.Events;

namespace MusicianFinder.Application.Validators.Events
{
    /// <summary>
    /// Валидатор команды <see cref="UpdateEventCommand"/>.
    /// </summary>
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Идентификатор мероприятия обязателен.");
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Название обязательно.");
            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Адрес не должен превышать 200 символов.");
            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).When(x => x.StartDateTime != default)
                .WithMessage("Дата начала должна быть в будущем.");
            RuleFor(x => x.EndDateTime)
                .GreaterThan(x => x.StartDateTime).When(x => x.EndDateTime.HasValue && x.StartDateTime != default)
                .WithMessage("Дата окончания должна быть позже даты начала.");
        }
    }
}