using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы со справочником регионов.
    /// </summary>
    public interface IRegionRepository
    {
        /// <summary>
        /// Получить все регионы с возможностью фильтрации и сортировки.
        /// </summary>
        /// <param name="query">Строка поиска по названию.</param>
        /// <param name="sortBy">Поле для сортировки (name, localizedname).</param>
        /// <param name="sortDesc">Направление сортировки (true — по убыванию).</param>
        /// <returns>Список регионов.</returns>
        Task<List<Region>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);

        /// <summary>
        /// Получить регион по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор региона.</param>
        /// <returns>Регион или null, если не найден.</returns>
        Task<Region?> GetByIdAsync(int id);
    }
}