using FluentValidation;
using MusicianFinder.Application.Commands.Profiles;

namespace MusicianFinder.Application.Validators.Profiles
{
    /// <summary>
    /// Валидатор команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotNull().WithMessage("Полное имя обязательно.");
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("Город обязателен.");
            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).WithMessage("Опыт не может быть отрицательным.");
            RuleFor(x => x.Phone)
                .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Телефон не должен превышать 20 символов.");
            RuleFor(x => x.Telegram)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Telegram))
                .WithMessage("Telegram не должен превышать 50 символов.");
        }
    }
}