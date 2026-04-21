using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Features.Uploads.DTOs;

namespace MusicianFinder.Application.Features.Uploads.UploadPhoto
{
    /// <summary>
    /// Команда для загрузки фото в портфолио.
    /// </summary>
    public class UploadPhotoCommand : IRequest<UploadResultDto>
    {
        /// <summary>
        /// Поток с фото.
        /// </summary>
        public Stream FileStream { get; set; } = null!;

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME-тип файла.
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание.
        /// </summary>
        public string? Description { get; set; }
    }
}