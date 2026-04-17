using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Profiles.UpdateAvatar
{
    /// <summary>
    /// Команда для обновления аватара профиля.
    /// </summary>
    public class UpdateAvatarCommand : IRequest<string>
    {
        /// <summary>
        /// Поток с изображением.
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
    }
}