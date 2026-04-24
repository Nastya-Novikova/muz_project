using MediatR;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Запрос для получения количества непрочитанных уведомлений.
    /// </summary>
    public class GetUnreadCountQuery : IQuery<int>
    {
    }
}