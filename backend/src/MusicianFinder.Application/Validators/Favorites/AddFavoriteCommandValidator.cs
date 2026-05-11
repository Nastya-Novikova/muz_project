using FluentValidation;
using MusicianFinder.Application.Commands.Favorites;

namespace MusicianFinder.Application.Validators.Favorites
{
    /// <summary>
    /// Валидатор команды <see cref="AddFavoriteCommand"/>.
    /// </summary>
    public class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public AddFavoriteCommandValidator()
        {
            RuleFor(x => x.TargetProfileId)
                .NotEmpty().WithMessage("Идентификатор профиля обязателен.");
        }
    }
}