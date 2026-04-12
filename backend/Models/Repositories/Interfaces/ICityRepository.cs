using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface ICityRepository
{
    Task<List<City>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
    Task<City?> GetByIdAsync(int id);
}