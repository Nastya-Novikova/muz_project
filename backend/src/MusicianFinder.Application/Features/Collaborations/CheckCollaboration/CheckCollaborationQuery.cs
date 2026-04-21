using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Collaborations.CheckCollaboration
{
    /// <summary>
    /// Запрос для проверки, отправлялось ли предложение указанному профилю.
    /// </summary>
    public class CheckCollaborationQuery : IRequest<bool>
    {
        /// <summary>
        /// Идентификатор проверяемого профиля.
        /// </summary>
        public Guid CollaboratedProfileId { get; set; }
    }
}