using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с фотографиями портфолио.
    /// </summary>
    public interface IPortfolioPhotoRepository
    {
        /// <summary>
        /// Добавить фотографию.
        /// </summary>
        /// <param name="photo">Фотография для добавления.</param>
        Task AddAsync(PortfolioPhoto photo);

        /// <summary>
        /// Получить все фотографии указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Список фотографий.</returns>
        Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId);

        /// <summary>
        /// Получить фотографию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор фотографии.</param>
        /// <returns>Фотография или null, если не найдена.</returns>
        Task<PortfolioPhoto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Удалить фотографию.
        /// </summary>
        /// <param name="id">Идентификатор фотографии.</param>
        Task RemoveAsync(Guid id);
    }
}