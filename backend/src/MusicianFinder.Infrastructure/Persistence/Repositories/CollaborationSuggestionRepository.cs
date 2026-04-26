using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces.Repositories;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация репозитория для записи предложений о сотрудничестве.
    /// </summary>
    public class CollaborationSuggestionRepository : ICollaborationSuggestionRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CollaborationSuggestionRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public CollaborationSuggestionRepository(AppDbContext dbContext) => _dbContext = dbContext;

        /// <inheritdoc />
        public async Task<CollaborationSuggestion?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbContext.CollaborationSuggestions.FirstOrDefaultAsync(cs => cs.Id == id, ct);

        /// <inheritdoc />
        public void Add(CollaborationSuggestion suggestion) => _dbContext.CollaborationSuggestions.Add(suggestion);
    }
}