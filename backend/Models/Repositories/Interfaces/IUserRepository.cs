using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByMusicianProfileIdAsync(Guid profileId);
    Task UpdateAsync(User user);
    Task SoftDeleteAsync(Guid id);
}