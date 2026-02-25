using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MusicianFinderDbContext _context;
    //public DbSet<User> Users { get; set; }

    public UserRepository(MusicianFinderDbContext context)
    {
        _context = context;
        //Users = _context.Set<User>();
    }

    public async Task AddAsync(User user)
    {
        if (user == null)
            throw new ApiException(400, "ѕользователь не может быть null", "USER_IS_NULL");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ApiException(400, "Email об€зателен", "MISSING_EMAIL");

        await _context.Users.AddAsync(user);
        //await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID пользовател€ не может быть пустым", "INVALID_USER_ID");

        return await _context.Users.Include(u => u.MusicianProfile).FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ApiException(400, "Email не может быть пустым", "MISSING_EMAIL");

        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task UpdateAsync(User user)
    {
        if (user == null)
            throw new ApiException(400, "ѕользователь не может быть null", "USER_IS_NULL");

        if (user.Id == Guid.Empty)
            throw new ApiException(400, "Ќекорректный ID пользовател€", "INVALID_USER_ID");

        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
            throw new ApiException(404, "ѕользователь не найден", "USER_NOT_FOUND");

        _context.Users.Update(user);
        //await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID пользовател€ не может быть пустым", "INVALID_USER_ID");

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new ApiException(404, "ѕользователь не найден", "USER_NOT_FOUND");

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        //await _context.SaveChangesAsync();
    }
}