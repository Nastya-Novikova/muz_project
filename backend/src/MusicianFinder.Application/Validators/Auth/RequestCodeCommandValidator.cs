using FluentValidation;
using MusicianFinder.Application.Commands.Auth;

namespace MusicianFinder.Application.Validators.Auth
{
    /// <summary>
    /// Валидатор команды <see cref="RequestCodeCommand"/>.
    /// </summary>
    public class RequestCodeCommandValidator : AbstractValidator<RequestCodeCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RequestCodeCommandValidator"/>.
        /// </summary>
        public RequestCodeCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен.")
                .EmailAddress().WithMessage("Некорректный email.");
        }
    }
}