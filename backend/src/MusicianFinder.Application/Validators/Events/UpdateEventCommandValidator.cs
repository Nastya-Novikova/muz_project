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
            RuleFor(x => x.EventId).NotEmpty();

            When(x => !string.IsNullOrEmpty(x.Title), () =>
                RuleFor(x => x.Title).MaximumLength(200));
            When(x => !string.IsNullOrEmpty(x.Address), () =>
                RuleFor(x => x.Address).MaximumLength(200));
            When(x => x.StartDateTime.HasValue, () =>
                RuleFor(x => x.StartDateTime).GreaterThan(DateTime.UtcNow));
            When(x => x.EndDateTime.HasValue && x.StartDateTime.HasValue, () =>
                RuleFor(x => x.EndDateTime).GreaterThan(x => x.StartDateTime));
            When(x => x.MaxParticipants.HasValue, () =>
                RuleFor(x => x.MaxParticipants).GreaterThanOrEqualTo(0));
        }
    }
}