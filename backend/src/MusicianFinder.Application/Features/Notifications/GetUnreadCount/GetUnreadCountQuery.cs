using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Notifications.GetUnreadCount
{
    /// <summary>
    /// Запрос для получения количества непрочитанных уведомлений.
    /// </summary>
    public class GetUnreadCountQuery : IRequest<int>
    {
    }
}