using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Collaborations.SendSuggestion
{
    /// <summary>
    /// Валидатор команды <see cref="SendSuggestionCommand"/>.
    /// </summary>
    public class SendSuggestionCommandValidator : AbstractValidator<SendSuggestionCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SendSuggestionCommandValidator"/>.
        /// </summary>
        public SendSuggestionCommandValidator()
        {
            RuleFor(x => x.ToProfileId)
                .NotEmpty().WithMessage("ID получателя обязателен.");

            RuleFor(x => x.Message)
                .MaximumLength(500).WithMessage("Сообщение не должно превышать 500 символов.");
        }
    }
}