using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация репозитория для записи пользователей.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public UserRepository(AppDbContext dbContext) => _dbContext = dbContext;

        /// <inheritdoc />
        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default)
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct);

        /// <inheritdoc />
        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

        /// <inheritdoc />
        public void Add(User user) => _dbContext.Users.Add(user);
    }
}