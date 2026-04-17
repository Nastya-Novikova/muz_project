using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Auth.RequestCode
{
    /// <summary>
    /// Обработчик команды <see cref="RequestCodeCommand"/>.
    /// </summary>
    public class RequestCodeCommandHandler : IRequestHandler<RequestCodeCommand>
    {
        private readonly IEmailVerificationCodeRepository _codeRepository;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RequestCodeCommandHandler"/>.
        /// </summary>
        /// <param name="codeRepository">Репозиторий кодов подтверждения.</param>
        /// <param name="emailService">Сервис отправки email.</param>
        public RequestCodeCommandHandler(
            IEmailVerificationCodeRepository codeRepository,
            IEmailService emailService)
        {
            _codeRepository = codeRepository;
            _emailService = emailService;
        }

        /// <inheritdoc />
        public async Task Handle(RequestCodeCommand request, CancellationToken cancellationToken)
        {
            var code = GenerateCode();
            var verificationCode = new EmailVerificationCode(request.Email, code);

            await _codeRepository.AddAsync(verificationCode);
            await _emailService.SendVerificationCodeAsync(request.Email, code);
        }

        private static string GenerateCode()
        {
            // В production — случайный 6-значный код
            // Для тестового окружения можно использовать фиксированный
            return "111111";
        }
    }
}