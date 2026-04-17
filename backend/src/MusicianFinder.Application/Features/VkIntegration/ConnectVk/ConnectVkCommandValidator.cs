using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace MusicianFinder.Application.Features.VkIntegration.ConnectVk
{
    /// <summary>
    /// Валидатор команды <see cref="ConnectVkCommand"/>.
    /// </summary>
    public class ConnectVkCommandValidator : AbstractValidator<ConnectVkCommand>
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ConnectVkCommandValidator"/>.
        /// </summary>
        public ConnectVkCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код авторизации обязателен.");

            RuleFor(x => x.CodeVerifier)
                .NotEmpty().WithMessage("Верификатор кода обязателен.");

            RuleFor(x => x.DeviceId)
                .NotEmpty().WithMessage("Идентификатор устройства обязателен.");
        }
    }
}