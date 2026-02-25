using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;
using backend.Models.Enums;

namespace backend.Models.Repositories.Interfaces;

public interface IProfileRepository
{
    //DbSet<MusicianProfile> MusicianProfiles { get; }
    Task<(List<MusicianProfile> Items, int TotalCount)> SearchAsync(
            string? query = null,
            int? cityId = null,
            List<int>? genreIds = null,
            List<int>? specialtyIds = null,
            List<int>? goalIds = null,
            List<int>? desiredGenreIds = null,
            List<int>? desiredSpecialtyIds = null,
            LookingFor? lookingFor = null,
            ProfileType? profileType = null,
            int? experienceMin = null,
            int? experienceMax = null,
            int page = 1,
            int limit = 20,
            string? sortBy = "createdAt",
            bool sortDesc = true);


    Task<MusicianProfile?> GetByIdAsync(Guid id);
    Task<MusicianProfile?> GetByUserIdAsync(Guid userId);
    Task AddAsync(MusicianProfile profile);
    Task UpdateAsync(MusicianProfile profile);
    Task SoftDeleteAsync(Guid id);
    Task<List<MusicianProfile>> GetProfilesByIdsAsync(List<Guid> ids);
}