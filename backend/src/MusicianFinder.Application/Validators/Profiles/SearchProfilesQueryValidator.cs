using FluentValidation;
using MusicianFinder.Application.Queries.Profiles;

namespace MusicianFinder.Application.Validators.Profiles
{
    /// <summary>
    /// Валидатор запроса <see cref="SearchProfilesQuery"/>.
    /// </summary>
    public class SearchProfilesQueryValidator : AbstractValidator<SearchProfilesQuery>
    {
        /// <summary>
        /// Инициализирует новый экземпляр валидатора.
        /// </summary>
        public SearchProfilesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть >= 1.");
            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100.");
        }
    }
}