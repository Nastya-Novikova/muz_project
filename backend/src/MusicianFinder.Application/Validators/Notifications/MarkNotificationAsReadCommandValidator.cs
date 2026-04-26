using FluentValidation;
using MusicianFinder.Application.Commands.Notifications;

namespace MusicianFinder.Application.Validators.Notifications
{
    /// <summary>
    /// Валидатор команды <see cref="MarkNotificationAsReadCommand"/>.
    /// </summary>
    public class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
    {
        public MarkNotificationAsReadCommandValidator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty().WithMessage("Идентификатор уведомления обязателен.");
        }
    }
}