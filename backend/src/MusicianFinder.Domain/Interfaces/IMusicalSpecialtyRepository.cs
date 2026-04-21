using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы со справочником музыкальных специальностей.
    /// </summary>
    public interface IMusicalSpecialtyRepository
    {
        /// <summary>
        /// Получить все специальности с возможностью фильтрации и сортировки.
        /// </summary>
        /// <param name="query">Строка поиска по названию.</param>
        /// <param name="sortBy">Поле для сортировки (name, localizedname).</param>
        /// <param name="sortDesc">Направление сортировки (true — по убыванию).</param>
        /// <returns>Список специальностей.</returns>
        Task<List<MusicalSpecialty>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);

        /// <summary>
        /// Получить специальность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор специальности.</param>
        /// <returns>Специальность или null, если не найдена.</returns>
        Task<MusicalSpecialty?> GetByIdAsync(int id);

        /// <summary>
        /// Получить список специальностей по их идентификаторам.
        /// </summary>
        /// <param name="ids">Список идентификаторов.</param>
        /// <returns>Список найденных специальностей.</returns>
        Task<List<MusicalSpecialty>> GetByIdsAsync(List<int> ids);
    }
}