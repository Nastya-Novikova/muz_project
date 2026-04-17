using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Domain.Interfaces
{
    /// <summary>
    /// Репозиторий для работы со справочником целей сотрудничества.
    /// </summary>
    public interface ICollaborationGoalRepository
    {
        /// <summary>
        /// Получить все цели сотрудничества с возможностью фильтрации и сортировки.
        /// </summary>
        /// <param name="query">Строка поиска по названию.</param>
        /// <param name="sortBy">Поле для сортировки (name, localizedname).</param>
        /// <param name="sortDesc">Направление сортировки (true — по убыванию).</param>
        /// <returns>Список целей сотрудничества.</returns>
        Task<List<CollaborationGoal>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);

        /// <summary>
        /// Получить цель сотрудничества по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор цели.</param>
        /// <returns>Цель сотрудничества или null, если не найдена.</returns>
        Task<CollaborationGoal?> GetByIdAsync(int id);

        /// <summary>
        /// Получить список целей сотрудничества по их идентификаторам.
        /// </summary>
        /// <param name="ids">Список идентификаторов.</param>
        /// <returns>Список найденных целей.</returns>
        Task<List<CollaborationGoal>> GetByIdsAsync(List<int> ids);
    }
}
