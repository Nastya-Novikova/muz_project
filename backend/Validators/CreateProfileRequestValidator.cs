using backend.Models.DTOs.Profiles;
using FluentValidation;

namespace backend.Validators
{
    public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
    {
        public CreateProfileRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Полное имя обязательно")
                .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("Город обязателен");

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).WithMessage("Опыт не может быть отрицательным");

            RuleFor(x => x.Age)
                .InclusiveBetween(0, 150).When(x => x.Age.HasValue)
                .WithMessage("Возраст должен быть от 0 до 150 лет");
        }
    }
}
