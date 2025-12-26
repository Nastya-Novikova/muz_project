using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IProfileRepository
{
    DbSet<MusicianProfile> MusicianProfiles { get; }

    // Поиск с фильтрацией, сортировкой и пагинацией
    Task<(List<MusicianProfile> Profiles, int Total)> SearchAsync(
        string? query = null,
        int? cityId = null,
        List<int>? genreIds = null,
        List<int>? specialtyIds = null,
        List<int>? goalIds = null,
        int? experienceMin = null,
        int? experienceMax = null,
        int page = 1,
        int limit = 20,
        string? sortBy = "createdAt",
        bool sortDesc = true);

    Task<MusicianProfile?> GetByIdAsync(Guid id);
    //Task<MusicianProfile?> GetByUserIdAsync(Guid userId);
    Task AddAsync(MusicianProfile profile);
    Task UpdateAsync(MusicianProfile profile);
    Task SoftDeleteAsync(Guid id);
    Task SaveChanges();
    Task<List<MusicianProfile>> GetProfilesByIdsAsync(List<Guid> ids);
}