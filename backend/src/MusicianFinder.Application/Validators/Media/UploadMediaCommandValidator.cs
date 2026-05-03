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

        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
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

            RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content-Type обязателен.")
            .Must((cmd, ct) => IsValidContentType(cmd.Type, ct))
            .WithMessage("Content-Type не соответствует выбранному типу медиа.");

            RuleFor(x => x.FileName)
                .Must((cmd, name) => IsValidExtension(cmd.Type, name))
                .WithMessage("Расширение файла не соответствует выбранному типу.");
        }

        private static bool IsValidContentType(MediaType type, string contentType)
        {
            return type switch
            {
                MediaType.Audio => contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase),
                MediaType.Video => contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase),
                MediaType.Photo => contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        private static bool IsValidExtension(MediaType type, string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) return false;
            return type switch
            {
                MediaType.Audio => new[] { ".mp3", ".wav", ".ogg", ".flac", ".aac" }.Contains(ext),
                MediaType.Video => new[] { ".mp4", ".mov", ".avi", ".mkv" }.Contains(ext),
                MediaType.Photo => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }.Contains(ext),
                _ => false
            };
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