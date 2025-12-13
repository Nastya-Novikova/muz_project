namespace backend.Models.Repositories
{
    public interface IProfileRepository
    {
        Task<List<MusicianProfile>> SearchAsync(string? city = null, string? genre = null);
        Task<MusicianProfile?> GetByIdAsync(Guid id);
        Task AddAsync(MusicianProfile profile);
        Task DeleteAsync(Guid id);
    }
}
