using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IPortfolioVideoRepository
{
    Task AddAsync(PortfolioVideo video);
    Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId);
    Task<PortfolioVideo?> GetByIdAsync(Guid id);
    Task RemoveAsync(Guid id);
}