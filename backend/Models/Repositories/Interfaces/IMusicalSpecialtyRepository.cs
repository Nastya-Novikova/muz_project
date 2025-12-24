using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IMusicalSpecialtyRepository
{
    DbSet<MusicalSpecialty> MusicalSpecialties { get; }
    Task<List<MusicalSpecialty>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
    Task<MusicalSpecialty?> GetByIdAsync(int id);
}