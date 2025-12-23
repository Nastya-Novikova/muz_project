using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IEmailVerificationCodeRepository
{
    DbSet<EmailVerificationCode> EmailVerificationCodes { get; }
    Task AddAsync(EmailVerificationCode code);
    Task<EmailVerificationCode?> GetByCodeAndEmailAsync(string code, string email);
    Task MarkAsUsedAsync(Guid id);
}