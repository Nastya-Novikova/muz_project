using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Models.Repositories;

public class PortfolioVideoRepository : IPortfolioVideoRepository
{
    private readonly MusicianFinderDbContext _context;
    //public DbSet<PortfolioVideo> VideoFiles { get; set; }

    public PortfolioVideoRepository(MusicianFinderDbContext context)
    {
        _context = context;
        //VideoFiles = _context.Set<PortfolioVideo>();
    }

    public async Task AddAsync(PortfolioVideo video)
    {
        if (video.ProfileId == Guid.Empty)
            throw new ApiException(400, "ProfileID обязателен", "MISSING_PROFILE_ID");

        await _context.PortfolioVideo.AddAsync(video);
        //await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioVideo>> GetByProfileIdAsync(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ApiException(400, "ID профиля не может быть пустым", "INVALID_PROFILE_ID");

        return await _context.PortfolioVideo.Where(a => a.ProfileId == profileId).IgnoreAutoIncludes().ToListAsync();
    }

    public async Task<PortfolioVideo?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID видео не может быть пустым", "INVALID_VIDEO_ID");

        return await _context.PortfolioVideo.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID видео не может быть пустым", "INVALID_VIDEO_ID");

        var video = await _context.PortfolioVideo.FindAsync(id);
        if (video == null)
            throw new ApiException(404, "Видео не найдено", "VIDEO_NOT_FOUND");

        _context.PortfolioVideo.Remove(video);
    }
}