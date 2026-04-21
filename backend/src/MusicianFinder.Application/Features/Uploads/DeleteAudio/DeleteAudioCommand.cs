using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Uploads.DeleteAudio
{
    /// <summary>
    /// Команда для удаления аудиозаписи из портфолио.
    /// </summary>
    public class DeleteAudioCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор аудиозаписи.
        /// </summary>
        public Guid Id { get; set; }
    }
}