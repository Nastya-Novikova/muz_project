using FluentValidation;
using MusicianFinder.Application.Commands.Favorites;

namespace MusicianFinder.Application.Validators.Favorites
{
    /// <summary>
    /// Валидатор команды <see cref="RemoveFavoriteCommand"/>.
    /// </summary>
    public class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public RemoveFavoriteCommandValidator()
        {
            RuleFor(x => x.TargetProfileId)
                .NotEmpty().WithMessage("Идентификатор профиля обязателен.");
        }
    }
}