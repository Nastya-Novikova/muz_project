using FluentValidation;
using backend.Models.DTOs.Auth;

namespace backend.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный email");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код обязателен")
                .Length(6).WithMessage("Код должен содержать 6 символов");
        }
    }
}
