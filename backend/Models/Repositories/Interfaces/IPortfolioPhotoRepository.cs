using backend.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories.Interfaces;

public interface IPortfolioPhotoRepository
{
    DbSet<PortfolioPhoto> Photos { get; }
    Task AddAsync(PortfolioPhoto photo);
    Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId);
    Task<PortfolioPhoto?> GetByIdAsync(Guid id);
    Task RemoveAsync(Guid id);
}