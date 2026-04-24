using FluentValidation;
using MusicianFinder.Application.Commands.Notifications;

namespace MusicianFinder.Application.Validators.Notifications
{
    /// <summary>
    /// Валидатор команды <see cref="MarkAllNotificationsAsReadCommand"/>.
    /// </summary>
    public class MarkAllNotificationsAsReadCommandValidator : AbstractValidator<MarkAllNotificationsAsReadCommand>
    {
        public MarkAllNotificationsAsReadCommandValidator()
        {
        }
    }
}