using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IUserRepository
{
    DbSet<User> Users { get; }
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
    Task SoftDeleteAsync(Guid id);

    Task<List<User>> GetUsersByIdsAsync(List<Guid> ids);
}