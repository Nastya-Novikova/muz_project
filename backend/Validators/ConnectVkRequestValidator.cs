using backend.Models.DTOs.Vk;
using FluentValidation;

namespace backend.Validators
{
    public class ConnectVkRequestValidator : AbstractValidator<ConnectVkRequest>
    {
        public ConnectVkRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код авторизации обязателен");

            RuleFor(x => x.CodeVerifier)
                .NotEmpty().WithMessage("Верификатор кода обязателен");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Идентификатор устройства обязателен");
        }
    }
}
