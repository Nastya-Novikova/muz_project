using System;

namespace MusicianFinder.Domain.Entities
{
    /// <summary>
    /// Код подтверждения email.
    /// </summary>
    public class EmailVerificationCode
    {
        private EmailVerificationCode()
        {
            Email = string.Empty;
            Code = string.Empty;
        }

        /// <summary>
        /// Инициализирует новый экземпляр кода подтверждения.
        /// </summary>
        /// <param name="email">Email, для которого сгенерирован код.</param>
        /// <param name="code">Шестизначный код.</param>
        public EmailVerificationCode(string email, string code)
        {
            Id = Guid.NewGuid();
            Email = email;
            Code = code;
            CreatedAt = DateTime.UtcNow;
            IsUsed = false;
        }

        /// <summary>
        /// Идентификатор кода.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Email, для которого сгенерирован код.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Шестизначный код.
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

        /// <summary>
        /// Помечает код как использованный.
        /// </summary>
        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        /// <summary>
        /// Проверяет, истёк ли срок действия кода.
        /// </summary>
        /// <param name="validityPeriod">Период действия.</param>
        /// <returns>true, если код просрочен.</returns>
        public bool IsExpired(TimeSpan validityPeriod)
        {
            return DateTime.UtcNow > CreatedAt + validityPeriod;
        }
    }
}