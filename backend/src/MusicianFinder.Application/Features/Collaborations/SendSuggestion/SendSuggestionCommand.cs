using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Collaborations.SendSuggestion
{
    /// <summary>
    /// Команда для отправки предложения о сотрудничестве.
    /// </summary>
    public class SendSuggestionCommand : IRequest
    {
        /// <summary>
        /// Идентификатор профиля получателя.
        /// </summary>
        public Guid ToProfileId { get; set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        public string? Message { get; set; }
    }
}