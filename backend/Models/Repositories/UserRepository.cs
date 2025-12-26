using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<User> Users { get; set; }

    public UserRepository(MusicianFinderDbContext context)
    {
        _context = context;
        Users = _context.Set<User>();
    }

    public async Task AddAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ApiException(400, "Email обязателен", "MISSING_EMAIL");

        await Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await Users.Include(u => u.MusicianProfile).FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task UpdateAsync(User user)
    {
        if (user.Id == Guid.Empty)
            throw new ApiException(400, "Некорректный ID пользователя", "INVALID_USER_ID");

        Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var user = await Users.FindAsync(id);
        if (user == null)
            throw new ApiException(404, "Пользователь не найден", "USER_NOT_FOUND");

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        Users.Update(user);
        await _context.SaveChangesAsync();
    }
}