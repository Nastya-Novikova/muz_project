using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
{
    private readonly MusicianFinderDbContext _context;
    //public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

    public EmailVerificationCodeRepository(MusicianFinderDbContext context)
    {
        _context = context;
        //EmailVerificationCodes = _context.Set<EmailVerificationCode>();
    }

    public async Task AddAsync(EmailVerificationCode code)
    {
        if (string.IsNullOrWhiteSpace(code.Email) || string.IsNullOrWhiteSpace(code.Code))
            throw new ApiException(400, "Email и код обязательны", "MISSING_CODE_DATA");

        await _context.EmailVerificationCodes.AddAsync(code);
        //await _context.SaveChangesAsync();
    }

    public async Task<EmailVerificationCode?> GetByCodeAndEmailAsync(string code, string email)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(email))
            throw new ApiException(400, "Email и код обязательны", "MISSING_CODE_DATA");

        return await _context.EmailVerificationCodes
            .Where(c => c.Code == code && c.Email == email && !c.IsUsed)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task MarkAsUsedAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID кода не может быть пустым", "INVALID_CODE_ID");

        var code = await _context.EmailVerificationCodes.FindAsync(id);
        if (code == null)
            throw new ApiException(404, "Код подтверждения не найден", "CODE_NOT_FOUND");

        code.IsUsed = true;
        _context.EmailVerificationCodes.Update(code);
    }
}