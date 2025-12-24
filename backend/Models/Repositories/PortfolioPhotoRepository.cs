using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class PortfolioPhotoRepository : IPortfolioPhotoRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<PortfolioPhoto> Photos { get; set; }

    public PortfolioPhotoRepository(MusicianFinderDbContext context)
    {
        _context = context;
        Photos = _context.Set<PortfolioPhoto>();
    }

    public async Task AddAsync(PortfolioPhoto photo)
    {
        await Photos.AddAsync(photo);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId)
    {
        return await Photos.Where(p => p.ProfileId == profileId).ToListAsync();
    }
}