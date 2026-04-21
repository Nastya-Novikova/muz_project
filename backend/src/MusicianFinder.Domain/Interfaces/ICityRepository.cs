using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы со справочником городов.
    /// </summary>
    public interface ICityRepository
    {
        /// <summary>
        /// Получить все города с возможностью фильтрации и сортировки.
        /// </summary>
        /// <param name="query">Строка поиска по названию.</param>
        /// <param name="sortBy">Поле для сортировки (name, localizedname).</param>
        /// <param name="sortDesc">Направление сортировки (true — по убыванию).</param>
        /// <returns>Список городов.</returns>
        Task<List<City>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);

        /// <summary>
        /// Получить город по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор города.</param>
        /// <returns>Город или null, если не найден.</returns>
        Task<City?> GetByIdAsync(int id);
    }
}
