using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Auth.Login
{
    /// <summary>
    /// Валидатор команды <see cref="LoginCommand"/>.
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LoginCommandValidator"/>.
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