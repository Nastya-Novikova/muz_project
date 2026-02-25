using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class PortfolioAudioRepository : IPortfolioAudioRepository
{
    private readonly MusicianFinderDbContext _context;
    //public DbSet<PortfolioAudio> AudioFiles { get; set; }

    public PortfolioAudioRepository(MusicianFinderDbContext context)
    {
        _context = context;
        //AudioFiles = _context.Set<PortfolioAudio>();
    }

    public async Task AddAsync(PortfolioAudio audio)
    {
        if (audio.ProfileId == Guid.Empty)
            throw new ApiException(400, "ProfileID обязателен", "MISSING_PROFILE_ID");

        await _context.PortfolioAudio.AddAsync(audio);
        //await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioAudio>> GetByProfileIdAsync(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ApiException(400, "ID профиля не может быть пустым", "INVALID_PROFILE_ID");

        return await _context.PortfolioAudio.Where(a => a.ProfileId == profileId).OrderByDescending(a => a.CreatedAt).IgnoreAutoIncludes().ToListAsync();
    }

    public async Task<PortfolioAudio?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID аудио не может быть пустым", "INVALID_AUDIO_ID");

        return await _context.PortfolioAudio.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID аудио не может быть пустым", "INVALID_AUDIO_ID");

        var audio = await _context.PortfolioAudio.FindAsync(id);
        if (audio == null)
            throw new ApiException(404, "Аудио не найдено", "AUDIO_NOT_FOUND");

        _context.PortfolioAudio.Remove(audio);
    }
}