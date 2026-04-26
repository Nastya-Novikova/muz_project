using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces.ReadRepositories;

namespace MusicianFinder.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Реализация read-репозитория для справочных данных.
    /// </summary>
    public class ReferenceDataReadRepository : IReferenceDataReadRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceDataReadRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="mapper">Маппер.</param>
        public ReferenceDataReadRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> GetCitiesAsync(CancellationToken ct = default)
            => await _dbContext.Cities.AsNoTracking().ProjectTo<LookupItemDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> GetRegionsAsync(CancellationToken ct = default)
            => await _dbContext.Regions.AsNoTracking().ProjectTo<LookupItemDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> GetGenresAsync(CancellationToken ct = default)
            => await _dbContext.Genres.AsNoTracking().ProjectTo<LookupItemDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> GetSpecialtiesAsync(CancellationToken ct = default)
            => await _dbContext.MusicalSpecialties.AsNoTracking().ProjectTo<LookupItemDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> GetCollaborationGoalsAsync(CancellationToken ct = default)
            => await _dbContext.CollaborationGoals.AsNoTracking().ProjectTo<LookupItemDto>(_mapper.ConfigurationProvider).ToListAsync(ct);
    }
}