using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы с профилями музыкантов.
    /// </summary>
    public interface IProfileRepository
    {
        /// <summary>
        /// Выполнить поиск профилей с фильтрацией и пагинацией.
        /// </summary>
        /// <param name="query">Поисковый запрос по полному имени.</param>
        /// <param name="cityId">Фильтр по городу.</param>
        /// <param name="genreIds">Фильтр по предлагаемым жанрам.</param>
        /// <param name="specialtyIds">Фильтр по предлагаемым специальностям.</param>
        /// <param name="goalIds">Фильтр по целям сотрудничества.</param>
        /// <param name="desiredGenreIds">Фильтр по искомым жанрам.</param>
        /// <param name="desiredSpecialtyIds">Фильтр по искомым специальностям.</param>
        /// <param name="lookingFor">Фильтр по статусу поиска.</param>
        /// <param name="profileType">Фильтр по типу профиля.</param>
        /// <param name="experienceMin">Минимальный опыт.</param>
        /// <param name="experienceMax">Максимальный опыт.</param>
        /// <param name="page">Номер страницы.</param>
        /// <param name="limit">Размер страницы.</param>
        /// <param name="sortBy">Поле сортировки (fullname, age, experience, city, createdat).</param>
        /// <param name="sortDesc">Направление сортировки.</param>
        /// <returns>Кортеж: список профилей и общее количество.</returns>
        Task<(List<MusicianProfile> Items, int TotalCount)> SearchAsync(
            string? query = null,
            int? cityId = null,
            List<int>? genreIds = null,
            List<int>? specialtyIds = null,
            List<int>? goalIds = null,
            List<int>? desiredGenreIds = null,
            List<int>? desiredSpecialtyIds = null,
            LookingFor? lookingFor = null,
            ProfileType? profileType = null,
            int? experienceMin = null,
            int? experienceMax = null,
            int page = 1,
            int limit = 20,
            string? sortBy = "createdAt",
            bool sortDesc = true);

        /// <summary>
        /// Получить профиль по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор профиля.</param>
        /// <returns>Профиль или null, если не найден.</returns>
        Task<MusicianProfile?> GetByIdAsync(Guid id);

        /// <summary>
        /// Получить профиль по идентификатору пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Профиль или null, если не найден.</returns>
        Task<MusicianProfile?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Добавить новый профиль.
        /// </summary>
        /// <param name="profile">Профиль для добавления.</param>
        Task AddAsync(MusicianProfile profile);

        /// <summary>
        /// Обновить существующий профиль.
        /// </summary>
        /// <param name="profile">Профиль с обновлёнными данными.</param>
        Task UpdateAsync(MusicianProfile profile);

        /// <summary>
        /// Мягко удалить профиль по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор профиля.</param>
        Task SoftDeleteAsync(Guid id);

        /// <summary>
        /// Получить список профилей по их идентификаторам.
        /// </summary>
        /// <param name="ids">Список идентификаторов.</param>
        /// <returns>Список найденных профилей.</returns>
        Task<List<MusicianProfile>> GetProfilesByIdsAsync(List<Guid> ids);
    }
}