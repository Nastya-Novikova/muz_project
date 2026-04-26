using MediatR;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Notifications
{
    public class GetNotificationSettingsQuery : IQuery<NotificationSettingsDto>
    {
    }
}