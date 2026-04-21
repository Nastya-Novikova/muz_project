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

namespace MusicianFinder.Application.Features.Profiles.CreateProfile
{
    /// <summary>
    /// Обработчик команды <see cref="CreateProfileCommand"/>.
    /// </summary>
    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Guid>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IMusicalSpecialtyRepository _specialtyRepository;
        private readonly ICollaborationGoalRepository _goalRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CreateProfileCommandHandler"/>.
        /// </summary>
        /// <param name="profileRepository">Репозиторий профилей.</param>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="genreRepository">Репозиторий жанров.</param>
        /// <param name="specialtyRepository">Репозиторий специальностей.</param>
        /// <param name="goalRepository">Репозиторий целей сотрудничества.</param>
        /// <param name="currentUserService">Сервис текущего пользователя.</param>
        public CreateProfileCommandHandler(
            IProfileRepository profileRepository,
            IUserRepository userRepository,
            IGenreRepository genreRepository,
            IMusicalSpecialtyRepository specialtyRepository,
            ICollaborationGoalRepository goalRepository,
            ICurrentUserService currentUserService)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _genreRepository = genreRepository;
            _specialtyRepository = specialtyRepository;
            _goalRepository = goalRepository;
            _currentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId);
            if (user == null)
                throw new NotFoundException(nameof(User), _currentUserService.UserId);

            if (user.ProfileCreated)
                throw new ConflictException("Профиль уже создан.");

            var profile = new MusicianProfile(
                request.ProfileType,
                request.FullName,
                request.CityId,
                user.Email,
                request.Experience,
                request.LookingFor)
            {
                Age = request.Age,
                Description = request.Description,
                Phone = request.Phone,
                Telegram = request.Telegram
            };

            await LoadAndAddGenresAsync(profile, request.GenreIds);
            await LoadAndAddSpecialtiesAsync(profile, request.SpecialtyIds);
            await LoadAndAddCollaborationGoalsAsync(profile, request.CollaborationGoalIds);
            await LoadAndAddDesiredGenresAsync(profile, request.DesiredGenreIds);
            await LoadAndAddDesiredSpecialtiesAsync(profile, request.DesiredSpecialtyIds);

            await _profileRepository.AddAsync(profile);
            user.MarkProfileAsCreated(profile);
            await _userRepository.UpdateAsync(user);

            return profile.Id;
        }

        private async Task LoadAndAddGenresAsync(MusicianProfile profile, List<int>? genreIds)
        {
            if (genreIds == null || genreIds.Count == 0)
                return;

            var genres = await _genreRepository.GetByIdsAsync(genreIds);
            if (genres.Count != genreIds.Count)
            {
                var missing = genreIds.Except(genres.Select(g => g.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(CreateProfileCommand.GenreIds),
                        $"Жанры с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            foreach (var genre in genres)
                profile.AddGenre(genre);
        }

        private async Task LoadAndAddSpecialtiesAsync(MusicianProfile profile, List<int>? specialtyIds)
        {
            if (specialtyIds == null || specialtyIds.Count == 0)
                return;

            var specialties = await _specialtyRepository.GetByIdsAsync(specialtyIds);
            if (specialties.Count != specialtyIds.Count)
            {
                var missing = specialtyIds.Except(specialties.Select(s => s.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(CreateProfileCommand.SpecialtyIds),
                        $"Специальности с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            foreach (var specialty in specialties)
                profile.AddSpecialty(specialty);
        }

        private async Task LoadAndAddCollaborationGoalsAsync(MusicianProfile profile, List<int>? goalIds)
        {
            if (goalIds == null || goalIds.Count == 0)
                return;

            var goals = await _goalRepository.GetByIdsAsync(goalIds);
            if (goals.Count != goalIds.Count)
            {
                var missing = goalIds.Except(goals.Select(g => g.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(CreateProfileCommand.CollaborationGoalIds),
                        $"Цели сотрудничества с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            foreach (var goal in goals)
                profile.AddCollaborationGoal(goal);
        }

        private async Task LoadAndAddDesiredGenresAsync(MusicianProfile profile, List<int>? desiredGenreIds)
        {
            if (desiredGenreIds == null || desiredGenreIds.Count == 0)
                return;

            var genres = await _genreRepository.GetByIdsAsync(desiredGenreIds);
            if (genres.Count != desiredGenreIds.Count)
            {
                var missing = desiredGenreIds.Except(genres.Select(g => g.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(CreateProfileCommand.DesiredGenreIds),
                        $"Искомые жанры с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            foreach (var genre in genres)
                profile.AddDesiredGenre(genre);
        }

        private async Task LoadAndAddDesiredSpecialtiesAsync(MusicianProfile profile, List<int>? desiredSpecialtyIds)
        {
            if (desiredSpecialtyIds == null || desiredSpecialtyIds.Count == 0)
                return;

            var specialties = await _specialtyRepository.GetByIdsAsync(desiredSpecialtyIds);
            if (specialties.Count != desiredSpecialtyIds.Count)
            {
                var missing = desiredSpecialtyIds.Except(specialties.Select(s => s.Id)).ToList();
                throw new ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(CreateProfileCommand.DesiredSpecialtyIds),
                        $"Искомые специальности с ID [{string.Join(", ", missing)}] не найдены.")
                });
            }

            foreach (var specialty in specialties)
                profile.AddDesiredSpecialty(specialty);
        }
    }
}