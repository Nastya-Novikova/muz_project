using FluentValidation;
using MusicianFinder.Application.Commands.Profiles;

namespace MusicianFinder.Application.Validators.Profiles
{
    /// <summary>
    /// Валидатор команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.FullName).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FullName));
            RuleFor(x => x.CityId).NotEmpty().When(x => x.CityId.HasValue);
            RuleFor(x => x.Experience).GreaterThanOrEqualTo(0).When(x => x.Experience.HasValue);
            RuleFor(x => x.Phone).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone));
            RuleFor(x => x.Telegram).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Telegram));
        }
    }
}