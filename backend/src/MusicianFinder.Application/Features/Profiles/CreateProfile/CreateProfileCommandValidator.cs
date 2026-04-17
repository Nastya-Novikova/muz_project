using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Profiles.CreateProfile
{
    /// <summary>
    /// Валидатор команды <see cref="CreateProfileCommand"/>.
    /// </summary>
    public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateProfileCommandValidator"/>.
        /// </summary>
        public CreateProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Полное имя обязательно.")
                .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов.");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("Город обязателен.");

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).WithMessage("Опыт не может быть отрицательным.");

            RuleFor(x => x.Age)
                .InclusiveBetween(0, 150).When(x => x.Age.HasValue)
                .WithMessage("Возраст должен быть от 0 до 150 лет.");
        }
    }
}