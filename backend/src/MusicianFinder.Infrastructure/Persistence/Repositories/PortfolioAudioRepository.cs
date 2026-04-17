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
    /// Репозиторий для работы с аудиозаписями портфолио.
    /// </summary>
    public class PortfolioAudioRepository : IPortfolioAudioRepository
    {
        private readonly MusicianFinderDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="PortfolioAudioRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public PortfolioAudioRepository(MusicianFinderDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(PortfolioAudio audio)
        {
            await _context.PortfolioAudio.AddAsync(audio);
        }

        /// <inheritdoc />
        public async Task<List<PortfolioAudio>> GetByProfileIdAsync(Guid profileId)
        {
            return await _context.PortfolioAudio
                .Where(a => a.ProfileId == profileId)
                .OrderByDescending(a => a.CreatedAt)
                .IgnoreAutoIncludes()
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<PortfolioAudio?> GetByIdAsync(Guid id)
        {
            return await _context.PortfolioAudio.FindAsync(id);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(Guid id)
        {
            var audio = await _context.PortfolioAudio.FindAsync(id);
            if (audio != null)
                _context.PortfolioAudio.Remove(audio);
        }
    }
}