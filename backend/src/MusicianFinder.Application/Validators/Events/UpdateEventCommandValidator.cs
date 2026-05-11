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

            When(x => !string.IsNullOrEmpty(x.Title), () =>
                RuleFor(x => x.Title).MaximumLength(200).WithMessage("Название не должно превышать 200 символов."));

            When(x => !string.IsNullOrEmpty(x.Address), () =>
                RuleFor(x => x.Address).MaximumLength(200).WithMessage("Адрес не должен превышать 200 символов."));

            When(x => x.StartDateTime.HasValue, () =>
                RuleFor(x => x.StartDateTime).GreaterThan(DateTime.UtcNow).WithMessage("Дата начала должна быть в будущем."));

            When(x => x.EndDateTime.HasValue && x.StartDateTime.HasValue, () =>
                RuleFor(x => x.EndDateTime).GreaterThan(x => x.StartDateTime)
                    .WithMessage("Дата окончания должна быть позже даты начала."));

            When(x => x.MaxParticipants.HasValue, () =>
                RuleFor(x => x.MaxParticipants).GreaterThanOrEqualTo(0).WithMessage("Количество участников не может быть отрицательным."));
        }
    }
}