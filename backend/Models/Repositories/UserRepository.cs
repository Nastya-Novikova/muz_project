using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

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
        await Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateAsync(User user)
    {
        Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var user = await Users.FindAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetUsersByIdsAsync(List<Guid> ids)
    {
        return await Users.Where(u => ids.Contains(u.Id)).ToListAsync();
    }
}