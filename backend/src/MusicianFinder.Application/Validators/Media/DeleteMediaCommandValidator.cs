using FluentValidation;
using MusicianFinder.Application.Commands.Media;

namespace MusicianFinder.Application.Validators.Media
{
    /// <summary>
    /// Валидатор команды <see cref="DeleteMediaCommand"/>.
    /// </summary>
    public class DeleteMediaCommandValidator : AbstractValidator<DeleteMediaCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public DeleteMediaCommandValidator()
        {
            RuleFor(x => x.MediaId)
                .NotEmpty().WithMessage("Идентификатор медиа обязателен.");
        }
    }
}