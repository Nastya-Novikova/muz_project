using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.Events.GetEvents
{
    /// <summary>
    /// Валидатор запроса <see cref="GetEventsQuery"/>.
    /// </summary>
    public class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
    {
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
                .Must(value => value == null || new[] { "title", "startdatetime", "createdat" }.Contains(value.ToLower()))
                .WithMessage("Недопустимое поле сортировки. Допустимые значения: title, startdatetime, createdat.");
        }
    }
}