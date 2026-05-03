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
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FullName))
                .WithMessage("Имя не должно превышать 100 символов.");

            RuleFor(x => x.CityId)
                .NotEmpty().When(x => x.CityId.HasValue)
                .WithMessage("Город обязателен.");

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).When(x => x.Experience.HasValue)
                .WithMessage("Опыт не может быть отрицательным.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Телефон не должен превышать 20 символов.");

            RuleFor(x => x.Telegram)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Telegram))
                .WithMessage("Telegram не должен превышать 50 символов.");

            RuleFor(x => x.ProfileType)
                .IsInEnum().When(x => x.ProfileType.HasValue)
                .WithMessage("Недопустимый тип профиля.");

        }
    }
}