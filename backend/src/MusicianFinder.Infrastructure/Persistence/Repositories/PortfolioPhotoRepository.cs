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
    /// Репозиторий для работы с фотографиями портфолио.
    /// </summary>
    public class PortfolioPhotoRepository : IPortfolioPhotoRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="PortfolioPhotoRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public PortfolioPhotoRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(PortfolioPhoto photo)
        {
            await _context.PortfolioPhotos.AddAsync(photo);
        }

        /// <inheritdoc />
        public async Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId)
        {
            return await _context.PortfolioPhotos
                .Where(p => p.ProfileId == profileId)
                .OrderByDescending(p => p.CreatedAt)
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<PortfolioPhoto?> GetByIdAsync(Guid id)
        {
            return await _context.PortfolioPhotos.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(Guid id)
        {
            var photo = await _context.PortfolioPhotos.FindAsync(id);
            if (photo != null)
                _context.PortfolioPhotos.Remove(photo);
        }
    }
}