using backend.Models.DTOs.Auth;
using FluentValidation;

namespace backend.Validators
{
    public class RequestCodeRequestValidator : AbstractValidator<RequestCodeRequest>
    {
        public RequestCodeRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный email");
        }
    }
}
