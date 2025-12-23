using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories;

public class PortfolioAudioRepository : IPortfolioAudioRepository
{
    private readonly MusicianFinderDbContext _context;
    public DbSet<PortfolioAudio> AudioFiles { get; set; }

    public PortfolioAudioRepository(MusicianFinderDbContext context)
    {
        _context = context;
        AudioFiles = _context.Set<PortfolioAudio>();
    }

    public async Task AddAsync(PortfolioAudio audio)
    {
        await AudioFiles.AddAsync(audio);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioAudio>> GetByProfileIdAsync(Guid profileId)
    {
        return await AudioFiles.Where(a => a.ProfileId == profileId).ToListAsync();
    }

    public async Task<PortfolioAudio?> GetByIdAsync(Guid id)
    {
        return await AudioFiles.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var audio = await AudioFiles.FindAsync(id);
        if (audio != null)
        {
            AudioFiles.Remove(audio);
            await _context.SaveChangesAsync();
        }
    }
}