using Microsoft.EntityFrameworkCore;
using backend.Models.Classes;

namespace backend.Models.Repositories.Interfaces;

public interface IPortfolioAudioRepository
{
    Task AddAsync(PortfolioAudio audio);
    Task<List<PortfolioAudio>> GetByProfileIdAsync(Guid profileId);
    Task<PortfolioAudio?> GetByIdAsync(Guid id);
    Task RemoveAsync(Guid id);
}