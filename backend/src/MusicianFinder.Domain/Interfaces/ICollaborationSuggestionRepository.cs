using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с предложениями о сотрудничестве.
    /// </summary>
    public interface ICollaborationSuggestionRepository
    {
        /// <summary>
        /// Добавить новое предложение о сотрудничестве.
        /// </summary>
        /// <param name="suggestion">Предложение для добавления.</param>
        Task AddAsync(CollaborationSuggestion suggestion);

        /// <summary>
        /// Получить предложения, полученные указанным профилем.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя-получателя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="sortBy">Поле сортировки (createdAt, status).</param>
        /// <param name="sortDesc">Направление сортировки.</param>
        /// <returns>Список входящих предложений.</returns>
        Task<List<CollaborationSuggestion>> GetReceivedAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true);

        /// <summary>
        /// Получить предложения, отправленные указанным профилем.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя-отправителя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="sortBy">Поле сортировки (createdAt, status).</param>
        /// <param name="sortDesc">Направление сортировки.</param>
        /// <returns>Список исходящих предложений.</returns>
        Task<List<CollaborationSuggestion>> GetSentAsync(Guid userId, int page = 1, int limit = 20, string? sortBy = "createdAt", bool sortDesc = true);

        /// <summary>
        /// Получить предложение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор предложения.</param>
        /// <returns>Предложение или null, если не найдено.</returns>
        Task<CollaborationSuggestion?> GetByIdAsync(Guid id);

        /// <summary>
        /// Обновить существующее предложение.
        /// </summary>
        /// <param name="suggestion">Предложение с обновлёнными данными.</param>
        Task UpdateAsync(CollaborationSuggestion suggestion);

        /// <summary>
        /// Проверить, существует ли предложение от указанного отправителя к указанному получателю.
        /// </summary>
        /// <param name="fromProfileId">Идентификатор профиля отправителя.</param>
        /// <param name="toProfileId">Идентификатор профиля получателя.</param>
        /// <returns>true, если хотя бы одно предложение существует.</returns>
        Task<bool> ExistsAsync(Guid fromProfileId, Guid toProfileId);
    }
}
