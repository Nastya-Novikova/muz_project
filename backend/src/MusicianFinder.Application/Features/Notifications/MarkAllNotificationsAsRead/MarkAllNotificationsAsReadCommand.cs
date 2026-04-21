using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Notifications.MarkAllNotificationsAsRead
{
    /// <summary>
    /// Команда для отметки всех уведомлений как прочитанных.
    /// </summary>
    public class MarkAllNotificationsAsReadCommand : IRequest<Unit>
    {
    }
}