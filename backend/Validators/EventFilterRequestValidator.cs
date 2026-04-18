using backend.Models.DTOs.Events;
using FluentValidation;

namespace backend.Validators
{
    public class EventFilterRequestValidator : AbstractValidator<EventFilterRequest>
    {
        public EventFilterRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть >= 1");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100");

            // Опционально: проверка допустимых полей сортировки
            RuleFor(x => x.SortBy)
                .Must(value => value == null || new[] { "title", "startdatetime", "createdat" }.Contains(value.ToLower()))
                .WithMessage("Недопустимое поле сортировки. Допустимые значения: title, startdatetime, createdat");
        }
    }
}
