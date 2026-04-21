using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Uploads.DeletePhoto
{
    /// <summary>
    /// Команда для удаления фото из портфолио.
    /// </summary>
    public class DeletePhotoCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор фотографии.
        /// </summary>
        public Guid Id { get; set; }
    }
}