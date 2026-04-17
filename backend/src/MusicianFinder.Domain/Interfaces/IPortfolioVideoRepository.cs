using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с видеозаписями портфолио.
    /// </summary>
    public interface IPortfolioVideoRepository
    {
        /// <summary>
        /// Добавить видеозапись.
        /// </summary>
        /// <param name="video">Видеозапись для добавления.</param>
        Task AddAsync(PortfolioVideo video);

        /// <summary>
        /// Получить все видеозаписи указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Список видеозаписей.</returns>
        Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId);

        /// <summary>
        /// Получить видеозапись по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор видеозаписи.</param>
        /// <returns>Видеозапись или null, если не найдена.</returns>
        Task<PortfolioVideo?> GetByIdAsync(Guid id);

        /// <summary>
        /// Удалить видеозапись.
        /// </summary>
        /// <param name="id">Идентификатор видеозаписи.</param>
        Task RemoveAsync(Guid id);
    }
}