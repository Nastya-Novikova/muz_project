using backend.Models.DTOs.Profiles;
using FluentValidation;

namespace backend.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть >= 1");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100");

            RuleFor(x => x.ExperienceMin)
                .GreaterThanOrEqualTo(0).When(x => x.ExperienceMin.HasValue)
                .WithMessage("Минимальный опыт не может быть отрицательным");

            RuleFor(x => x.ExperienceMax)
                .GreaterThanOrEqualTo(0).When(x => x.ExperienceMax.HasValue)
                .WithMessage("Максимальный опыт не может быть отрицательным")
                .GreaterThanOrEqualTo(x => x.ExperienceMin ?? 0).When(x => x.ExperienceMax.HasValue && x.ExperienceMin.HasValue)
                .WithMessage("Максимальный опыт должен быть >= минимального");
        }
    }
}
