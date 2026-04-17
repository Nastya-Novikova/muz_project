using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public UserRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        /// <inheritdoc />
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.MusicianProfile)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        /// <inheritdoc />
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        /// <inheritdoc />
        public async Task<User?> GetByMusicianProfileIdAsync(Guid profileId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.MusicianProfile != null && u.MusicianProfile.Id == profileId && !u.IsDeleted);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task SoftDeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.MarkAsDeleted();
                _context.Users.Update(user);
            }
        }
    }
}