using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы со справочником музыкальных жанров.
    /// </summary>
    public interface IGenreRepository
    {
        /// <summary>
        /// Получить все жанры с возможностью фильтрации и сортировки.
        /// </summary>
        /// <param name="query">Строка поиска по названию.</param>
        /// <param name="sortBy">Поле для сортировки (name, localizedname).</param>
        /// <param name="sortDesc">Направление сортировки (true — по убыванию).</param>
        /// <returns>Список жанров.</returns>
        Task<List<Genre>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);

        /// <summary>
        /// Получить жанр по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор жанра.</param>
        /// <returns>Жанр или null, если не найден.</returns>
        Task<Genre?> GetByIdAsync(int id);

        /// <summary>
        /// Получить список жанров по их идентификаторам.
        /// </summary>
        /// <param name="ids">Список идентификаторов.</param>
        /// <returns>Список найденных жанров.</returns>
        Task<List<Genre>> GetByIdsAsync(List<int> ids);
    }
}