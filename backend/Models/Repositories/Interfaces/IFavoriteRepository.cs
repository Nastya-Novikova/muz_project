using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task AddAsync(Favorite favorite);
        Task RemoveAsync(Guid userId, Guid profileId);
        Task<bool> ExistsAsync(Guid userId, Guid profileId);
        Task<List<MusicianProfile>> GetFavoritesByUserIdAsync(Guid userId, int page, int limit);
        Task<int> CountFavoritesByUserIdAsync(Guid userId);
    }
}
