using FluentValidation;
using MusicianFinder.Application.Commands.Suggestions;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Validators.Suggestions
{
    /// <summary>
    /// Валидатор команды <see cref="UpdateSuggestionStatusCommand"/>.
    /// </summary>
    public class UpdateSuggestionStatusCommandValidator : AbstractValidator<UpdateSuggestionStatusCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public UpdateSuggestionStatusCommandValidator()
        {
            RuleFor(x => x.SuggestionId)
                .NotEmpty().WithMessage("Идентификатор предложения обязателен.");

            RuleFor(x => x.Status)
                .Must(status => status == SuggestionStatus.Accepted || status == SuggestionStatus.Rejected)
                .WithMessage("Статус может быть только Accepted или Rejected.");
        }
    }
}