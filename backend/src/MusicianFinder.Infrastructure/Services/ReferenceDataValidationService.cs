using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Infrastructure.Persistence;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Реализация сервиса проверки существования элементов справочников
    /// через прямые запросы к базе данных.
    /// </summary>
    public class ReferenceDataValidationService : IReferenceDataValidationService
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceDataValidationService"/>.
        /// </summary>
        /// <param name="db">Контекст базы данных.</param>
        public ReferenceDataValidationService(AppDbContext db) => _db = db;

        /// <inheritdoc />
        public async Task<bool> CityExistsAsync(int cityId, CancellationToken ct = default)
            => await _db.Cities.AnyAsync(c => c.Id == cityId, ct);

        /// <inheritdoc />
        public async Task<bool> RegionExistsAsync(int regionId, CancellationToken ct = default)
            => await _db.Regions.AnyAsync(r => r.Id == regionId, ct);

        /// <inheritdoc />
        public async Task<bool> GenreExistsAsync(int genreId, CancellationToken ct = default)
            => await _db.Genres.AnyAsync(g => g.Id == genreId, ct);

        /// <inheritdoc />
        public async Task<bool> SpecialtyExistsAsync(int specialtyId, CancellationToken ct = default)
            => await _db.MusicalSpecialties.AnyAsync(s => s.Id == specialtyId, ct);

        /// <inheritdoc />
        public async Task<bool> CollaborationGoalExistsAsync(int goalId, CancellationToken ct = default)
            => await _db.CollaborationGoals.AnyAsync(cg => cg.Id == goalId, ct);
    }
}