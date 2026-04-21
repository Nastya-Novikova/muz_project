using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Uploads.UploadPhoto
{
    /// <summary>
    /// Валидатор команды <see cref="UploadPhotoCommand"/>.
    /// </summary>
    public class UploadPhotoCommandValidator : AbstractValidator<UploadPhotoCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UploadPhotoCommandValidator"/>.
        /// </summary>
        public UploadPhotoCommandValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Название не должно превышать 100 символов.");

            RuleFor(x => x.ContentType)
                .Must(ct => ct.StartsWith("image/")).WithMessage("Разрешены только изображения.");

            RuleFor(x => x.FileStream)
                .Must(fs => fs.Length <= 500 * 1024 * 1024).WithMessage("Файл слишком большой (макс. 500 МБ).");
        }
    }
}