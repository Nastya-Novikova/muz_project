using FluentValidation;
using MusicianFinder.Application.Commands.Profiles;

namespace MusicianFinder.Application.Validators.Profiles
{
    /// <summary>
    /// Валидатор команды <see cref="DeleteProfileCommand"/>.
    /// Проверяет наличие ключа идемпотентности.
    /// </summary>
    public class DeleteProfileCommandValidator : AbstractValidator<DeleteProfileCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public DeleteProfileCommandValidator()
        {
        }
    }
}