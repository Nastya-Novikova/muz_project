using FluentValidation;
using MusicianFinder.Application.Commands.Events;

namespace MusicianFinder.Application.Validators.Events
{
    /// <summary>
    /// Валидатор команды <see cref="CreateEventCommand"/>.
    /// </summary>
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Название обязательно.");
            RuleFor(x => x.RegionId)
                .NotEmpty().WithMessage("Регион обязателен.");
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("Город обязателен.");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен.")
                .MaximumLength(200).WithMessage("Адрес не должен превышать 200 символов.");
            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Дата начала должна быть в будущем.");
            RuleFor(x => x.EndDateTime)
                .GreaterThan(x => x.StartDateTime).When(x => x.EndDateTime.HasValue)
                .WithMessage("Дата окончания должна быть позже даты начала.");
            RuleFor(x => x.MaxParticipants)
                .GreaterThanOrEqualTo(0).WithMessage("Количество участников не может быть отрицательным.");
        }
    }
}