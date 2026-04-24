using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий для записи предложений о сотрудничестве.
    /// </summary>
    public interface ICollaborationSuggestionRepository
    {
        /// <summary>
        /// Получает предложение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор предложения.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Предложение или null.</returns>
        Task<CollaborationSuggestion?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Добавляет новое предложение.
        /// </summary>
        /// <param name="suggestion">Экземпляр предложения.</param>
        void Add(CollaborationSuggestion suggestion);
    }
}