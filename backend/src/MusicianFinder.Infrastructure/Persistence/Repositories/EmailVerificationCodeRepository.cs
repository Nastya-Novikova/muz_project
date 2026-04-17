using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы с кодами подтверждения email.
    /// </summary>
    public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EmailVerificationCodeRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public EmailVerificationCodeRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(EmailVerificationCode code)
        {
            await _context.EmailVerificationCodes.AddAsync(code);
        }

        /// <inheritdoc />
        public async Task<EmailVerificationCode?> GetByCodeAndEmailAsync(string code, string email)
        {
            return await _context.EmailVerificationCodes
                .Where(c => c.Code == code && c.Email == email && !c.IsUsed)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task MarkAsUsedAsync(Guid id)
        {
            var code = await _context.EmailVerificationCodes.FindAsync(id);
            if (code != null)
            {
                code.MarkAsUsed();
                _context.EmailVerificationCodes.Update(code);
            }
        }
    }
}