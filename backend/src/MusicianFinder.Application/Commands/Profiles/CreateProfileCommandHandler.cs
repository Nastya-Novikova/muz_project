using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Exceptions;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Обработчик команды <see cref="CreateProfileCommand"/>.
    /// </summary>
    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Guid>
    {
        private readonly IReadDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEntityExistenceValidator _existenceValidator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateProfileCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        /// <param name="existenceValidator">Сервис проверки существования сущностей.</param>
        public CreateProfileCommandHandler(
            IReadDbContext dbContext,
            ICurrentUserService currentUserService,
            IEntityExistenceValidator existenceValidator)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _existenceValidator = existenceValidator;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Пользователь не найден.");

            if (user.ProfileCreated)
                throw new DomainException("Профиль уже создан.");

            var genres = await _existenceValidator.LoadAndValidateAsync<Genre>(request.GenreIds, "Жанры");
            var specialties = await _existenceValidator.LoadAndValidateAsync<MusicalSpecialty>(request.SpecialtyIds, "Специальности");
            var goals = await _existenceValidator.LoadAndValidateAsync<CollaborationGoal>(request.CollaborationGoalIds, "Цели сотрудничества");
            var desiredGenres = await _existenceValidator.LoadAndValidateAsync<Genre>(request.DesiredGenreIds, "Искомые жанры");
            var desiredSpecialties = await _existenceValidator.LoadAndValidateAsync<MusicalSpecialty>(request.DesiredSpecialtyIds, "Искомые специальности");

            var profile = new MusicianProfile(
                request.ProfileType,
                request.FullName,
                request.CityId,
                user.Email,
                request.Experience,
                request.LookingFor);

            profile.UpdateBasicInfo(
                profileType: null,
                fullName: null,
                age: request.Age,
                description: request.Description,
                phone: request.Phone,
                telegram: request.Telegram,
                cityId: null,
                experience: null,
                lookingFor: null,
                notifyByEmail: null,
                notifyByVk: null);

            foreach (var g in genres) profile.AddGenre(g);
            foreach (var s in specialties) profile.AddSpecialty(s);
            foreach (var g in goals) profile.AddCollaborationGoal(g);
            foreach (var g in desiredGenres) profile.AddDesiredGenre(g);
            foreach (var s in desiredSpecialties) profile.AddDesiredSpecialty(s);

            await ((DbContext)_dbContext).AddAsync(profile, cancellationToken);
            user.MarkProfileAsCreated(profile);
            await ((DbContext)_dbContext).SaveChangesAsync(cancellationToken);

            return profile.Id;
        }
    }
}