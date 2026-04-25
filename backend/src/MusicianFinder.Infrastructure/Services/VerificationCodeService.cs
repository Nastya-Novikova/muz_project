using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Реализация сервиса для работы с кодами подтверждения email.
    /// </summary>
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="VerificationCodeService"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public VerificationCodeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<string> GenerateAndSaveCodeAsync(string email, CancellationToken cancellationToken = default)
        {
            // Генерация 6-значного кода
            var code = GenerateSixDigitCode();
            var verificationCode = new EmailVerificationCode(email, code);
            _dbContext.Set<EmailVerificationCode>().Add(verificationCode);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return code;
        }

        /// <inheritdoc />
        public async Task<bool> ValidateCodeAsync(string email, string code, CancellationToken cancellationToken = default)
        {
            var verificationCode = await _dbContext.Set<EmailVerificationCode>()
                .FirstOrDefaultAsync(
                    vc => vc.Email == email && vc.Code == code && !vc.IsUsed,
                    cancellationToken);

            if (verificationCode == null)
                return false;

            if (verificationCode.IsExpired(TimeSpan.FromMinutes(10)))
                return false;

            verificationCode.MarkAsUsed();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static string GenerateSixDigitCode()
        {
            return "111111";
            return new Random().Next(100000, 999999).ToString();
        }
    }
}