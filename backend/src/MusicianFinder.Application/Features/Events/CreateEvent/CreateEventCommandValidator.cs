using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Events.CreateEvent
{
    /// <summary>
    /// Валидатор команды <see cref="CreateEventCommand"/>.
    /// </summary>
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateEventCommandValidator"/>.
        /// </summary>
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название мероприятия обязательно.")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов.");

            RuleFor(x => x.RegionId)
                .GreaterThan(0).WithMessage("Регион обязателен.");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("Город обязателен.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес обязателен.")
                .MaximumLength(200).WithMessage("Адрес не должен превышать 200 символов.");

            RuleFor(x => x.StartDateTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Дата начала не может быть в прошлом.");

            RuleFor(x => x.EndDateTime)
                .GreaterThan(x => x.StartDateTime).When(x => x.EndDateTime.HasValue)
                .WithMessage("Дата окончания должна быть позже даты начала.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThanOrEqualTo(0).WithMessage("Количество участников не может быть отрицательным.");
        }
    }
}