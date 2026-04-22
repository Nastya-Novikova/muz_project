using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateProfileCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public UpdateProfileCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .Include(nameof(Domain.Entities.MusicianProfile.Genres))
                .Include(nameof(Domain.Entities.MusicianProfile.Specialties))
                .Include(nameof(Domain.Entities.MusicianProfile.CollaborationGoals))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredGenres))
                .Include(nameof(Domain.Entities.MusicianProfile.DesiredSpecialties))
                .FirstOrDefaultAsync(p => p.Id == _currentUserService.UserId && !p.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Профиль текущего пользователя не найден.");

            profile.UpdateBasicInfo(
                request.ProfileType,
                request.FullName,
                request.Age,
                request.Description,
                request.Phone,
                request.Telegram,
                request.CityId,
                request.Experience,
                request.LookingFor,
                request.NotifyByEmail,
                request.NotifyByVk);

            if (request.GenreIds is not null)
            {
                var genres = await _dbContext.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToListAsync(cancellationToken);
                profile.ClearGenres();
                foreach (var g in genres) profile.AddGenre(g);
            }

            if (request.SpecialtyIds is not null)
            {
                var specialties = await _dbContext.Specialties.Where(s => request.SpecialtyIds.Contains(s.Id)).ToListAsync(cancellationToken);
                profile.ClearSpecialties();
                foreach (var s in specialties) profile.AddSpecialty(s);
            }

            if (request.CollaborationGoalIds is not null)
            {
                var goals = await _dbContext.CollaborationGoals.Where(g => request.CollaborationGoalIds.Contains(g.Id)).ToListAsync(cancellationToken);
                profile.ClearCollaborationGoals();
                foreach (var g in goals) profile.AddCollaborationGoal(g);
            }

            if (request.DesiredGenreIds is not null)
            {
                var desiredGenres = await _dbContext.Genres.Where(g => request.DesiredGenreIds.Contains(g.Id)).ToListAsync(cancellationToken);
                profile.ClearDesiredGenres();
                foreach (var g in desiredGenres) profile.AddDesiredGenre(g);
            }

            if (request.DesiredSpecialtyIds is not null)
            {
                var desiredSpecialties = await _dbContext.Specialties.Where(s => request.DesiredSpecialtyIds.Contains(s.Id)).ToListAsync(cancellationToken);
                profile.ClearDesiredSpecialties();
                foreach (var s in desiredSpecialties) profile.AddDesiredSpecialty(s);
            }

            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}