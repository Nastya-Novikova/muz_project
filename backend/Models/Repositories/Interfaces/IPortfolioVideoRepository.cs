using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IPortfolioVideoRepository
{
    DbSet<PortfolioVideo> VideoFiles { get; }
    Task AddAsync(PortfolioVideo video);
    Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId);
    Task<PortfolioVideo?> GetByIdAsync(Guid id);
    Task RemoveAsync(Guid id);
}