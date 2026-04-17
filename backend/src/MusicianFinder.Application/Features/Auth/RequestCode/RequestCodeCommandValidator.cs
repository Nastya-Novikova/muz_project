using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Auth.RequestCode
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