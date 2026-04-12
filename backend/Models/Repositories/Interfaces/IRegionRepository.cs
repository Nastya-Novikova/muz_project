using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
        Task<Region?> GetByIdAsync(int id);
    }
}
