using backend.Models.DTOs.Collaborations;
using FluentValidation;

namespace backend.Validators
{
    public class SendSuggestionRequestValidator : AbstractValidator<SendSuggestionRequest>
    {
        public SendSuggestionRequestValidator()
        {
            RuleFor(x => x.ToProfileId)
                .NotEmpty().WithMessage("ID получателя обязателен");

            RuleFor(x => x.Message)
                .MaximumLength(500).WithMessage("Сообщение не должно превышать 500 символов");
        }
    }
}
