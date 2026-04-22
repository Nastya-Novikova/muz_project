using MediatR;

namespace MusicianFinder.Application.Queries.Notifications
{
    /// <summary>
    /// Запрос для получения количества непрочитанных уведомлений.
    /// </summary>
    public class GetUnreadCountQuery : IRequest<int>
    {
    }
}