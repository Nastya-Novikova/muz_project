using backend.Data;
using backend.Models.Repositories.Interfaces;

namespace backend.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MusicianFinderDbContext _context;

        public UnitOfWork(MusicianFinderDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
