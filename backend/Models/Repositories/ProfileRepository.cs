using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Models.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly MusicianFinderDbContext _context;
        public DbSet<MusicianProfile> MusicianProfiles { get; set; }

        public ProfileRepository(MusicianFinderDbContext context)
        {
            _context = context;
            MusicianProfiles = _context.Set<MusicianProfile>();
        }

        public async Task<List<MusicianProfile>> SearchAsync(string? city, string? genre)
        {
            var query = MusicianProfiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(p => p.City == city);

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(p => p.Genres.Contains(genre));

            return await query.ToListAsync();
        }

        public async Task<MusicianProfile?> GetByIdAsync(Guid id)
        {
            return await MusicianProfiles.FindAsync(id);
        }

        public async Task AddAsync(MusicianProfile profile)
        {
            MusicianProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var profile = await MusicianProfiles.FindAsync(id);
            if (profile != null)
            {
                MusicianProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }
    }
}
