using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;
using backend.Models.Classes;

namespace backend.Models.Repositories;

public class PortfolioPhotoRepository : IPortfolioPhotoRepository
{
    private readonly MusicianFinderDbContext _context;

    public PortfolioPhotoRepository(MusicianFinderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PortfolioPhoto photo)
    {
        await _context.PortfolioPhotos.AddAsync(photo);
    }

    public async Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId)
    {
        return await _context.PortfolioPhotos.Where(p => p.ProfileId == profileId).OrderByDescending(p => p.CreatedAt).IgnoreAutoIncludes().ToListAsync();
    }

    public async Task<PortfolioPhoto?> GetByIdAsync(Guid id)
    {
        return await _context.PortfolioPhotos.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var photo = await _context.PortfolioPhotos.FindAsync(id);

        _context.PortfolioPhotos.Remove(photo);
    }
}