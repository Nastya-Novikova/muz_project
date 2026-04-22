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
        /// Инициализирует новый экземпляр <see cref="UpdateProfileCommandValidator"/>.
        /// </summary>
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FullName))
                .WithMessage("Имя не должно превышать 100 символов.");

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).When(x => x.Experience.HasValue)
                .WithMessage("Опыт не может быть отрицательным.");

            RuleFor(x => x.Age)
                .InclusiveBetween(0, 150).When(x => x.Age.HasValue)
                .WithMessage("Возраст должен быть от 0 до 150 лет.");
        }
    }
}