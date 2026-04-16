using backend.Models.DTOs.Events;
using FluentValidation;

namespace backend.Validators
{
    public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
    {
        public UpdateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Название не должно превышать 200 символов");

            RuleFor(x => x.Address)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Address))
                .WithMessage("Адрес не должен превышать 200 символов");

            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).When(x => x.StartDateTime.HasValue)
                .WithMessage("Дата начала не может быть в прошлом");

            RuleFor(x => x.EndDateTime)
                .GreaterThan(x => x.StartDateTime).When(x => x.EndDateTime.HasValue && x.StartDateTime.HasValue)
                .WithMessage("Дата окончания должна быть позже даты начала");

            RuleFor(x => x.MaxParticipants)
                .InclusiveBetween(1, 1000).When(x => x.MaxParticipants.HasValue)
                .WithMessage("Количество участников должно быть от 1 до 1000");
        }
    }
}
