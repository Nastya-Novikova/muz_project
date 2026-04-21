using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с кодами подтверждения email.
    /// </summary>
    public interface IEmailVerificationCodeRepository
    {
        /// <summary>
        /// Добавить новый код подтверждения.
        /// </summary>
        /// <param name="code">Код подтверждения.</param>
        Task AddAsync(EmailVerificationCode code);

        /// <summary>
        /// Найти действительный код по email и значению кода.
        /// </summary>
        /// <param name="code">Значение кода.</param>
        /// <param name="email">Email, для которого был выпущен код.</param>
        /// <returns>Код подтверждения или null, если не найден или уже использован.</returns>
        Task<EmailVerificationCode?> GetByCodeAndEmailAsync(string code, string email);

        /// <summary>
        /// Пометить код как использованный.
        /// </summary>
        /// <param name="id">Идентификатор кода.</param>
        Task MarkAsUsedAsync(Guid id);
    }
}
