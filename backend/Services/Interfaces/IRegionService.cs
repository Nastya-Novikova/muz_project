using backend.Models.Common;
using backend.Models.DTOs;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы со справочником регионов
    /// </summary>
    public interface IRegionService
    {
        /// <summary>
        /// Получить все регионы с фильтрацией и сортировкой
        /// </summary>
        Task<Result<List<LookupItemDto>>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
    }
}
