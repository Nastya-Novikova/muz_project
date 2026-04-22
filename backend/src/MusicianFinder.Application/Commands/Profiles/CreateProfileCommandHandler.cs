using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Common.Exceptions;
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

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateProfileCommandHandler"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CreateProfileCommandHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
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

            var genres = request.GenreIds?.Count > 0
                ? await _dbContext.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToListAsync(cancellationToken)
                : [];
            var specialties = request.SpecialtyIds?.Count > 0
                ? await _dbContext.Specialties.Where(s => request.SpecialtyIds.Contains(s.Id)).ToListAsync(cancellationToken)
                : [];
            var goals = request.CollaborationGoalIds?.Count > 0
                ? await _dbContext.CollaborationGoals.Where(g => request.CollaborationGoalIds.Contains(g.Id)).ToListAsync(cancellationToken)
                : [];
            var desiredGenres = request.DesiredGenreIds?.Count > 0
                ? await _dbContext.Genres.Where(g => request.DesiredGenreIds.Contains(g.Id)).ToListAsync(cancellationToken)
                : [];
            var desiredSpecialties = request.DesiredSpecialtyIds?.Count > 0
                ? await _dbContext.Specialties.Where(s => request.DesiredSpecialtyIds.Contains(s.Id)).ToListAsync(cancellationToken)
                : [];

            var profile = new MusicianProfile(
                request.ProfileType,
                request.FullName,
                request.CityId,
                user.Email,
                request.Experience,
                request.LookingFor);

            // Устанавливаем опциональные свойства через методы или конструктор (здесь они уже переданы в конструктор с значениями по умолчанию)
            // Если нужны дополнительные поля, можно вызвать UpdateBasicInfo или передать в конструктор (расширить конструктор).
            // Для простоты оставим как есть, но в реальном проекте нужно расширить конструктор.
            // Здесь исправляем: вызываем UpdateBasicInfo для установки Age, Description, Phone, Telegram.
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