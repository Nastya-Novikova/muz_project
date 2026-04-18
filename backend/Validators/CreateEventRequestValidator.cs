using backend.Models.DTOs.Events;
using FluentValidation;

namespace backend.Validators
{
    public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
    {
        public CreateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название мероприятия обязательно")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

            RuleFor(x => x.RegionId)
                .GreaterThan(0).WithMessage("Регион обязателен");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("Город обязателен");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен")
                .MaximumLength(200).WithMessage("Адрес не должен превышать 200 символов");

            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Дата начала не может быть в прошлом");

            RuleFor(x => x.EndDateTime)
                .GreaterThan(x => x.StartDateTime).When(x => x.EndDateTime.HasValue)
                .WithMessage("Дата окончания должна быть позже даты начала");

            RuleFor(x => x.MaxParticipants)
                .InclusiveBetween(1, 1000).WithMessage("Количество участников должно быть от 1 до 1000");
        }
    }
}
