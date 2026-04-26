using FluentValidation;
using MusicianFinder.Application.Commands.Profiles;

namespace MusicianFinder.Application.Validators.Profiles
{
    /// <summary>
    /// Валидатор команды <see cref="UpdateAvatarCommand"/>.
    /// Проверяет наличие файла, его тип и размер.
    /// </summary>
    public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
    {
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public UpdateAvatarCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Файл аватара обязателен.")
                .Must(c => c.Length <= MaxFileSize)
                .WithMessage($"Размер файла не должен превышать {MaxFileSize / 1024 / 1024} МБ.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content-Type обязателен.")
                .Must(ct => AllowedContentTypes.Contains(ct))
                .WithMessage("Допустимы только изображения (JPEG, PNG, GIF).");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Имя файла обязательно.");
        }
    }
}