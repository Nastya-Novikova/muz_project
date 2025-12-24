using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories.Interfaces;

public interface IPortfolioPhotoRepository
{
    DbSet<PortfolioPhoto> Photos { get; }
    Task AddAsync(PortfolioPhoto photo);
    Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId);
}