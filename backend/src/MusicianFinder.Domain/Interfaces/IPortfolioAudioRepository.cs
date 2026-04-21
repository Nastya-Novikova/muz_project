using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с аудиозаписями портфолио.
    /// </summary>
    public interface IPortfolioAudioRepository
    {
        /// <summary>
        /// Добавить аудиозапись.
        /// </summary>
        /// <param name="audio">Аудиозапись для добавления.</param>
        Task AddAsync(PortfolioAudio audio);

        /// <summary>
        /// Получить все аудиозаписи указанного профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Список аудиозаписей.</returns>
        Task<List<PortfolioAudio>> GetByProfileIdAsync(Guid profileId);

        /// <summary>
        /// Получить аудиозапись по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор аудиозаписи.</param>
        /// <returns>Аудиозапись или null, если не найдена.</returns>
        Task<PortfolioAudio?> GetByIdAsync(Guid id);

        /// <summary>
        /// Удалить аудиозапись.
        /// </summary>
        /// <param name="id">Идентификатор аудиозаписи.</param>
        Task RemoveAsync(Guid id);
    }
}