using MediatR;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Обработчик команды <see cref="RequestCodeCommand"/>.
    /// </summary>
    public class RequestCodeCommandHandler : IRequestHandler<RequestCodeCommand, Unit>
    {
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeService _verificationCodeService;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="emailService">Сервис отправки писем.</param>
        /// <param name="verificationCodeService">Сервис для работы с кодами подтверждения.</param>
        public RequestCodeCommandHandler(
            IEmailService emailService,
            IVerificationCodeService verificationCodeService)
        {
            _emailService = emailService;
            _verificationCodeService = verificationCodeService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RequestCodeCommand request, CancellationToken cancellationToken)
        {
            var code = await _verificationCodeService.GenerateAndSaveCodeAsync(request.Email, cancellationToken);
            await _emailService.SendVerificationCodeAsync(request.Email, code);
            return Unit.Value;
        }
    }
}