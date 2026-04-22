using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Auth
{
    /// <summary>
    /// Обработчик команды <see cref="RequestCodeCommand"/>.
    /// </summary>
    public class RequestCodeCommandHandler : IRequestHandler<RequestCodeCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="RequestCodeCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="emailService">Сервис отправки email.</param>
        public RequestCodeCommandHandler(IReadDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(RequestCodeCommand request, CancellationToken cancellationToken)
        {
            var code = GenerateCode();
            var verificationCode = new EmailVerificationCode(request.Email, code);

            await ((DbContext)_dbContext).AddAsync(verificationCode, cancellationToken);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            await _emailService.SendVerificationCodeAsync(request.Email, code);

            return Unit.Value;
        }

        private static string GenerateCode()
        {
            return "111111";
        }
    }
}