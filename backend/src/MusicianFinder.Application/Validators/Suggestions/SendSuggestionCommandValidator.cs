using FluentValidation;
using MusicianFinder.Application.Commands.Suggestions;

namespace MusicianFinder.Application.Validators.Suggestions
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