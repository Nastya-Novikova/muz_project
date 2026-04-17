using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Код подтверждения email.
    /// </summary>
    public class EmailVerificationCode
    {
        private EmailVerificationCode() { }

        public EmailVerificationCode(string email, string code)
        {
            Id = Guid.NewGuid();
            Email = email;
            Code = code;
            CreatedAt = DateTime.UtcNow;
            IsUsed = false;
        }

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Email, для которого сгенерирован код.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// 6-значный код.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Время создания.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Использован ли код.
        /// </summary>
        public bool IsUsed { get; private set; }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        public bool IsExpired(TimeSpan validityPeriod)
        {
            return DateTime.UtcNow > CreatedAt + validityPeriod;
        }
    }
}
