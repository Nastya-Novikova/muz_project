using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Notifications.MarkNotificationAsRead
{
    /// <summary>
    /// Команда для отметки уведомления как прочитанного.
    /// </summary>
    public class MarkNotificationAsReadCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор уведомления.
        /// </summary>
        public Guid NotificationId { get; set; }
    }
}