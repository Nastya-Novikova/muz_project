using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Uploads.UploadAudio
{
    /// <summary>
    /// Валидатор команды <see cref="UploadAudioCommand"/>.
    /// </summary>
    public class UploadAudioCommandValidator : AbstractValidator<UploadAudioCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadAudioCommandValidator"/>.
        /// </summary>
        public UploadAudioCommandValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Название не должно превышать 100 символов.");

            RuleFor(x => x.ContentType)
                .Must(ct => ct.StartsWith("audio/")).WithMessage("Разрешены только аудиофайлы.");

            RuleFor(x => x.FileStream)
                .Must(fs => fs.Length <= 1000 * 1024 * 1024).WithMessage("Файл слишком большой (макс. 1000 МБ).");
        }
    }
}