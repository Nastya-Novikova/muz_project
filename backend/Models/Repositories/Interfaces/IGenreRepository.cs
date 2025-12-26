using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IGenreRepository
{
    DbSet<Genre> Genres { get; }
    Task<List<Genre>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false);
    Task<Genre?> GetByIdAsync(int id);
}