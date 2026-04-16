using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
{
    private readonly MusicianFinderDbContext _context;

    public EmailVerificationCodeRepository(MusicianFinderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EmailVerificationCode code)
    {
        await _context.EmailVerificationCodes.AddAsync(code);
    }

    public async Task<EmailVerificationCode?> GetByCodeAndEmailAsync(string code, string email)
    {
        return await _context.EmailVerificationCodes
            .Where(c => c.Code == code && c.Email == email && !c.IsUsed)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task MarkAsUsedAsync(Guid id)
    {
        var code = await _context.EmailVerificationCodes.FindAsync(id);

        code.IsUsed = true;
        _context.EmailVerificationCodes.Update(code);
    }
}