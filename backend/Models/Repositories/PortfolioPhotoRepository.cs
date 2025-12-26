using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;
using backend.Models.Classes;

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
        if (photo.ProfileId == Guid.Empty)
            throw new ApiException(400, "ProfileID обязателен", "MISSING_PROFILE_ID");

        await Photos.AddAsync(photo);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId)
    {
        return await Photos.Where(p => p.ProfileId == profileId).ToListAsync();
    }

    public async Task<PortfolioPhoto?> GetByIdAsync(Guid id)
    {
        return await Photos.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var photo = await Photos.FindAsync(id);
        if (photo != null)
        {
            Photos.Remove(photo);
            await _context.SaveChangesAsync();
        }
    }
}