using FluentValidation;
using MusicianFinder.Application.Commands.Auth;

namespace MusicianFinder.Application.Validators.Auth
{
    /// <summary>
    /// Валидатор команды <see cref="LoginCommand"/>.
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен.")
                .EmailAddress().WithMessage("Некорректный email.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код обязателен.")
                .Length(6).WithMessage("Код должен содержать 6 символов.");
        }
    }
}