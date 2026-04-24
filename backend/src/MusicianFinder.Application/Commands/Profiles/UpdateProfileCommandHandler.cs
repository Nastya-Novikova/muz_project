using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEntityExistenceValidator _existenceValidator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateProfileCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="existenceValidator">Сервис проверки существования сущностей.</param>
        public UpdateProfileCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService,
            IEntityExistenceValidator existenceValidator)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _existenceValidator = existenceValidator;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Profiles
                .Include(nameof(MusicianProfile.Genres))
                .Include(nameof(MusicianProfile.Specialties))
                .Include(nameof(MusicianProfile.CollaborationGoals))
                .Include(nameof(MusicianProfile.DesiredGenres))
                .Include(nameof(MusicianProfile.DesiredSpecialties))
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

            await ApplyEntitiesAsync<Genre>(request.GenreIds, "Жанры",
                profile.ClearGenres, g => profile.AddGenre(g), cancellationToken);
            await ApplyEntitiesAsync<MusicalSpecialty>(request.SpecialtyIds, "Специальности",
                profile.ClearSpecialties, s => profile.AddSpecialty(s), cancellationToken);
            await ApplyEntitiesAsync<CollaborationGoal>(request.CollaborationGoalIds, "Цели сотрудничества",
                profile.ClearCollaborationGoals, g => profile.AddCollaborationGoal(g), cancellationToken);
            await ApplyEntitiesAsync<Genre>(request.DesiredGenreIds, "Искомые жанры",
                profile.ClearDesiredGenres, g => profile.AddDesiredGenre(g), cancellationToken);
            await ApplyEntitiesAsync<MusicalSpecialty>(request.DesiredSpecialtyIds, "Искомые специальности",
                profile.ClearDesiredSpecialties, s => profile.AddDesiredSpecialty(s), cancellationToken);

            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        private async Task ApplyEntitiesAsync<T>(
            List<int>? ids,
            string entityName,
            Action clearAction,
            Action<T> addAction,
            CancellationToken cancellationToken)
            where T : class
        {
            if (ids is null) return;

            var entities = await _existenceValidator.LoadAndValidateAsync<T>(ids, entityName);
            clearAction();
            foreach (var entity in entities)
                addAction(entity);
        }
    }
}