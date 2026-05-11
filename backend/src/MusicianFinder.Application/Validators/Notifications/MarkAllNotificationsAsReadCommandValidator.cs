using FluentValidation;
using MusicianFinder.Application.Commands.Notifications;

namespace MusicianFinder.Application.Validators.Notifications
{
    /// <summary>
    /// Валидатор команды <see cref="MarkAllNotificationsAsReadCommand"/>.
    /// </summary>
    public class MarkAllNotificationsAsReadCommandValidator : AbstractValidator<MarkAllNotificationsAsReadCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public MarkAllNotificationsAsReadCommandValidator()
        {
        }
    }
}