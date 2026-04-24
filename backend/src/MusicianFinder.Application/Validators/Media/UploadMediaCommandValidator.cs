using FluentValidation;
using MusicianFinder.Application.Commands.Media;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Application.Validators.Media
{
    /// <summary>
    /// Валидатор команды <see cref="UploadMediaCommand"/>.
    /// Проверяет файл, название и тип медиа.
    /// </summary>
    public class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
    {
        private const long MaxAudioSize = 100 * 1024 * 1024; // 100 MB
        private const long MaxVideoSize = 500 * 1024 * 1024; // 500 MB
        private const long MaxPhotoSize = 5 * 1024 * 1024;   // 5 MB

        public UploadMediaCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Файл обязателен.")
                .Must((cmd, content) => content.Length <= GetMaxFileSize(cmd.Type))
                .WithMessage("Файл слишком большой для указанного типа медиа.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Имя файла обязательно.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название медиа обязательно.")
                .MaximumLength(100).WithMessage("Название не должно превышать 100 символов.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Указан недопустимый тип медиа.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content-Type обязателен.");
        }

        private static long GetMaxFileSize(MediaType type) => type switch
        {
            MediaType.Audio => MaxAudioSize,
            MediaType.Video => MaxVideoSize,
            MediaType.Photo => MaxPhotoSize,
            _ => 0
        };
    }
}