using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class PortfolioVideoRepository : IPortfolioVideoRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<PortfolioVideo> VideoFiles { get; set; }

    public PortfolioVideoRepository(MusicianFinderDbContext context)
    {
        _context = context;
        VideoFiles = _context.Set<PortfolioVideo>();
    }

    public async Task AddAsync(PortfolioVideo video)
    {
        if (video.ProfileId == Guid.Empty)
            throw new ApiException(400, "ProfileID обязателен", "MISSING_PROFILE_ID");

        await VideoFiles.AddAsync(video);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId)
    {
        return await VideoFiles.Where(a => a.ProfileId == profileId).ToListAsync();
    }

    public async Task<PortfolioVideo?> GetByIdAsync(Guid id)
    {
        return await VideoFiles.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var video = await VideoFiles.FindAsync(id);
        if (video != null)
        {
            VideoFiles.Remove(video);
            await _context.SaveChangesAsync();
        }
    }
}