// Без фильтрации — только базовые операции
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

    public EmailVerificationCodeRepository(MusicianFinderDbContext context)
    {
        _context = context;
        EmailVerificationCodes = _context.Set<EmailVerificationCode>();
    }

    public async Task AddAsync(EmailVerificationCode code)
    {
        await EmailVerificationCodes.AddAsync(code);
        await _context.SaveChangesAsync();
    }

    public async Task<EmailVerificationCode?> GetByCodeAndEmailAsync(string code, string email)
    {
        return await EmailVerificationCodes
            .Where(c => c.Code == code && c.Email == email && !c.IsUsed)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task MarkAsUsedAsync(Guid id)
    {
        var code = await EmailVerificationCodes.FindAsync(id);
        if (code != null)
        {
            code.IsUsed = true;
            EmailVerificationCodes.Update(code);
            await _context.SaveChangesAsync();
        }
    }
}