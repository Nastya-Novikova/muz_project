using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Common.Exceptions;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Interfaces;

namespace MusicianFinder.Application.Features.Profiles.UpdateProfile
{
    /// <summary>
    /// Обработчик команды <see cref="UpdateProfileCommand"/>.
    /// </summary>
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IMusicalSpecialtyRepository _specialtyRepository;
        private readonly ICollaborationGoalRepository _goalRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UpdateProfileCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="genreRepository">Репозиторий жанров.</param>
        /// <param name="specialtyRepository">Репозиторий специальностей.</param>
        /// <param name="goalRepository">Репозиторий целей сотрудничества.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public UpdateProfileCommandHandler(
            IProfileRepository profileRepository,
            IGenreRepository genreRepository,
            IMusicalSpecialtyRepository specialtyRepository,
            ICollaborationGoalRepository goalRepository,
            ICurrentUserService currentUserService)
        {
            _profileRepository = profileRepository;
            _genreRepository = genreRepository;
            _specialtyRepository = specialtyRepository;
            _goalRepository = goalRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetByUserIdAsync(_currentUserService.UserId);
            if (profile == null)
                throw new NotFoundException("Профиль текущего пользователя не найден.");

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

            await UpdateGenresAsync(profile, request.GenreIds);
            await UpdateSpecialtiesAsync(profile, request.SpecialtyIds);
            await UpdateCollaborationGoalsAsync(profile, request.CollaborationGoalIds);
            await UpdateDesiredGenresAsync(profile, request.DesiredGenreIds);
            await UpdateDesiredSpecialtiesAsync(profile, request.DesiredSpecialtyIds);

            await _profileRepository.UpdateAsync(profile);
            return Unit.Value;
        }

        private async Task UpdateGenresAsync(MusicianProfile profile, List<int>? genreIds)
        {
            if (genreIds == null)
                return;

            var genres = await _genreRepository.GetByIdsAsync(genreIds);
            if (genres.Count != genreIds.Count)
            {
                var missing = genreIds.Except(genres.Select(g => g.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(UpdateProfileCommand.GenreIds),
                        $"Жанры с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            profile.ClearGenres();
            foreach (var genre in genres)
                profile.AddGenre(genre);
        }

        private async Task UpdateSpecialtiesAsync(MusicianProfile profile, List<int>? specialtyIds)
        {
            if (specialtyIds == null)
                return;

            var specialties = await _specialtyRepository.GetByIdsAsync(specialtyIds);
            if (specialties.Count != specialtyIds.Count)
            {
                var missing = specialtyIds.Except(specialties.Select(s => s.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(UpdateProfileCommand.SpecialtyIds),
                        $"Специальности с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            profile.ClearSpecialties();
            foreach (var specialty in specialties)
                profile.AddSpecialty(specialty);
        }

        private async Task UpdateCollaborationGoalsAsync(MusicianProfile profile, List<int>? goalIds)
        {
            if (goalIds == null)
                return;

            var goals = await _goalRepository.GetByIdsAsync(goalIds);
            if (goals.Count != goalIds.Count)
            {
                var missing = goalIds.Except(goals.Select(g => g.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(UpdateProfileCommand.CollaborationGoalIds),
                        $"Цели сотрудничества с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            profile.ClearCollaborationGoals();
            foreach (var goal in goals)
                profile.AddCollaborationGoal(goal);
        }

        private async Task UpdateDesiredGenresAsync(MusicianProfile profile, List<int>? desiredGenreIds)
        {
            if (desiredGenreIds == null)
                return;

            var genres = await _genreRepository.GetByIdsAsync(desiredGenreIds);
            if (genres.Count != desiredGenreIds.Count)
            {
                var missing = desiredGenreIds.Except(genres.Select(g => g.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(UpdateProfileCommand.DesiredGenreIds),
                        $"Искомые жанры с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            profile.ClearDesiredGenres();
            foreach (var genre in genres)
                profile.AddDesiredGenre(genre);
        }

        private async Task UpdateDesiredSpecialtiesAsync(MusicianProfile profile, List<int>? desiredSpecialtyIds)
        {
            if (desiredSpecialtyIds == null)
                return;

            var specialties = await _specialtyRepository.GetByIdsAsync(desiredSpecialtyIds);
            if (specialties.Count != desiredSpecialtyIds.Count)
            {
                var missing = desiredSpecialtyIds.Except(specialties.Select(s => s.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(UpdateProfileCommand.DesiredSpecialtyIds),
                        $"Искомые специальности с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            profile.ClearDesiredSpecialties();
            foreach (var specialty in specialties)
                profile.AddDesiredSpecialty(specialty);
        }
    }
}