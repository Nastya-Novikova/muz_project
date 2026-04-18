using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class PortfolioAudioRepository : IPortfolioAudioRepository
{
    private readonly MusicianFinderDbContext _context;

    public PortfolioAudioRepository(MusicianFinderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PortfolioAudio audio)
    {
        await _context.PortfolioAudio.AddAsync(audio);
    }

    public async Task<List<PortfolioAudio>> GetByProfileIdAsync(Guid profileId)
    {
        return await _context.PortfolioAudio.Where(a => a.ProfileId == profileId).OrderByDescending(a => a.CreatedAt).IgnoreAutoIncludes().ToListAsync();
    }

    public async Task<PortfolioAudio?> GetByIdAsync(Guid id)
    {
        return await _context.PortfolioAudio.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        var audio = await _context.PortfolioAudio.FindAsync(id);

        _context.PortfolioAudio.Remove(audio);
    }
}