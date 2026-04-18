using backend.Models.DTOs.User;
using FluentValidation;

namespace backend.Validators
{
    public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
    {
        public UpdateUserProfileRequestValidator()
        {
            RuleFor(x => x.ProfileCreated)
                .NotNull().WithMessage("Поле ProfileCreated обязательно");
        }
    }
}
