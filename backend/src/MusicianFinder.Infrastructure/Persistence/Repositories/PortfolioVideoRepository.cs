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
    /// Репозиторий для работы с видеозаписями портфолио.
    /// </summary>
    public class PortfolioVideoRepository : IPortfolioVideoRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="PortfolioVideoRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public PortfolioVideoRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(PortfolioVideo video)
        {
            await _context.PortfolioVideo.AddAsync(video);
        }

        /// <inheritdoc />
        public async Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId)
        {
            return await _context.PortfolioVideo
                .Where(v => v.ProfileId == profileId)
                .OrderByDescending(v => v.CreatedAt)
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<PortfolioVideo?> GetByIdAsync(Guid id)
        {
            return await _context.PortfolioVideo.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(Guid id)
        {
            var video = await _context.PortfolioVideo.FindAsync(id);
            if (video != null)
                _context.PortfolioVideo.Remove(video);
        }
    }
}