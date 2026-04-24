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

        /// <summary>
        /// Инициализирует новый экземпляр обработчика.
        /// </summary>
        /// <param name="emailService">Сервис отправки писем.</param>
        public RequestCodeCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RequestCodeCommand request, CancellationToken cancellationToken)
        {
            var code = GenerateCode();
            await _emailService.SendVerificationCodeAsync(request.Email, code);
            return Unit.Value;
        }

        private static string GenerateCode() => "111111";
    }
}