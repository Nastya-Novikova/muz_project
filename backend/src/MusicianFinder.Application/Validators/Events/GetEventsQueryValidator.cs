using FluentValidation;
using MusicianFinder.Application.Queries.Events;

namespace MusicianFinder.Application.Validators.Events
{
    /// <summary>
    /// Валидатор запроса <see cref="GetEventsQuery"/>.
    /// </summary>
    public class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
    {
        private static readonly string[] _allowedSortFields = ["title", "startdatetime", "createdat"];

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetEventsQueryValidator"/>.
        /// </summary>
        public GetEventsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть >= 1.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100.");

            RuleFor(x => x.SortBy)
                .Must(value => value is null || _allowedSortFields.Contains(value.ToLower()))
                .WithMessage("Недопустимое поле сортировки. Допустимые значения: title, startdatetime, createdat.");
        }
    }
}