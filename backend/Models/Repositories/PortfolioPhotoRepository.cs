using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models.Repositories.Interfaces;
using backend.Exceptions;
using backend.Models.Classes;

namespace backend.Models.Repositories;

public class PortfolioPhotoRepository : IPortfolioPhotoRepository
{
    private readonly MusicianFinderDbContext _context;
    //public DbSet<PortfolioPhoto> Photos { get; set; }

    public PortfolioPhotoRepository(MusicianFinderDbContext context)
    {
        _context = context;
        //Photos = _context.Set<PortfolioPhoto>();
    }

    public async Task AddAsync(PortfolioPhoto photo)
    {
        if (photo.ProfileId == Guid.Empty)
            throw new ApiException(400, "ProfileID обязателен", "MISSING_PROFILE_ID");

        await _context.PortfolioPhotos.AddAsync(photo);
        //await _context.SaveChangesAsync();
    }

    public async Task<List<PortfolioPhoto>> GetByProfileIdAsync(Guid profileId)
    {
        if (profileId == Guid.Empty)
            throw new ApiException(400, "ID профиля не может быть пустым", "INVALID_PROFILE_ID");

        return await _context.PortfolioPhotos.Where(p => p.ProfileId == profileId).OrderByDescending(p => p.CreatedAt).IgnoreAutoIncludes().ToListAsync();
    }

    public async Task<PortfolioPhoto?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID фото не может быть пустым", "INVALID_PHOTO_ID");

        return await _context.PortfolioPhotos.FindAsync(id);
    }

    public async Task RemoveAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ApiException(400, "ID фото не может быть пустым", "INVALID_PHOTO_ID");

        var photo = await _context.PortfolioPhotos.FindAsync(id);
        if (photo == null)
            throw new ApiException(404, "Фото не найдено", "PHOTO_NOT_FOUND");

        _context.PortfolioPhotos.Remove(photo);
    }
}