using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Uploads.DeleteVideo
{
    /// <summary>
    /// Команда для удаления видеозаписи из портфолио.
    /// </summary>
    public class DeleteVideoCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор видеозаписи.
        /// </summary>
        public Guid Id { get; set; }
    }
}