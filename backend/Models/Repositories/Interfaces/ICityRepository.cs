using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface ICityRepository
{
    DbSet<City> Cities { get; }
    Task<List<City>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
    Task<City?> GetByIdAsync(int id);
    Task AddAsync(City city);
    Task UpdateAsync(City city);
}