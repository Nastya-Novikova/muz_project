using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с пользователями.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Добавить нового пользователя.
        /// </summary>
        /// <param name="user">Пользователь для добавления.</param>
        Task AddAsync(User user);

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Пользователь или null, если не найден.</returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить пользователя по email.
        /// </summary>
        /// <param name="email">Email пользователя.</param>
        /// <returns>Пользователь или null, если не найден.</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Получить пользователя по идентификатору его музыкального профиля.
        /// </summary>
        /// <param name="profileId">Идентификатор профиля музыканта.</param>
        /// <returns>Пользователь или null, если не найден.</returns>
        Task<User?> GetByMusicianProfileIdAsync(Guid profileId);

        /// <summary>
        /// Обновить существующего пользователя.
        /// </summary>
        /// <param name="user">Пользователь с обновлёнными данными.</param>
        Task UpdateAsync(User user);

        /// <summary>
        /// Мягко удалить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        Task SoftDeleteAsync(Guid id);
    }
}