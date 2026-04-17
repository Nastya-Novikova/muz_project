using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с избранными профилями.
    /// </summary>
    public interface IFavoriteRepository
    {
        /// <summary>
        /// Добавить профиль в избранное.
        /// </summary>
        /// <param name="favorite">Объект избранного.</param>
        Task AddAsync(Favorite favorite);

        /// <summary>
        /// Удалить профиль из избранного.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        Task RemoveAsync(Guid userId, Guid profileId);

        /// <summary>
        /// Проверить, находится ли профиль в избранном у пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>true, если профиль в избранном.</returns>
        Task<bool> ExistsAsync(Guid userId, Guid profileId);

        /// <summary>
        /// Получить список избранных профилей пользователя с пагинацией.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <returns>Список профилей музыкантов.</returns>
        Task<List<MusicianProfile>> GetFavoritesByUserIdAsync(Guid userId, int page, int limit);

        /// <summary>
        /// Получить общее количество избранных профилей пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Количество избранных профилей.</returns>
        Task<int> CountFavoritesByUserIdAsync(Guid userId);
    }
}