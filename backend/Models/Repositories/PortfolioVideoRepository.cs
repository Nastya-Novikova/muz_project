using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class PortfolioVideoRepository : IPortfolioVideoRepository
{
    private readonly MusicianFinderDbContext _context;

    public PortfolioVideoRepository(MusicianFinderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PortfolioVideo video)
    {
        await _context.PortfolioVideo.AddAsync(video);
    }

    public async Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId)
    {
        return await _context.PortfolioVideo.Where(a => a.ProfileId == profileId).IgnoreAutoIncludes().ToListAsync();
    }

    public async Task<PortfolioVideo?> GetByIdAsync(Guid id)
    {
        return await _context.PortfolioVideo.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var video = await _context.PortfolioVideo.FindAsync(id);

        _context.PortfolioVideo.Remove(video);
    }
}