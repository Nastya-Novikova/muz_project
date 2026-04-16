using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.Repositories;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class EntityExistenceService(
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        ICityRepository cityRepository,
        IRegionRepository regionRepository,
        IEventRepository eventRepository,
        IGenreRepository genreRepository,
        IMusicalSpecialtyRepository specialtyRepository,
        ICollaborationGoalRepository collaborationGoalRepository) : IEntityExistenceService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly ICityRepository _cityRepository = cityRepository;
        private readonly IRegionRepository _regionRepository = regionRepository;
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IGenreRepository _genreRepository = genreRepository;
        private readonly IMusicalSpecialtyRepository _specialtyRepository = specialtyRepository;
        private readonly ICollaborationGoalRepository _goalRepository = collaborationGoalRepository;

        public async Task<Result> ValidateUserWithProfileAsync(Guid userId)
        {
            var result = await GetUserWithProfileAsync(userId);
            return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
        }

        public async Task<Result> ValidateMusicianProfileAsync(Guid profileId)
        {
            var result = await GetMusicianProfileAsync(profileId);
            return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
        }

        public async Task<Result> ValidateCityAsync(int cityId)
        {
            var result = await GetCityAsync(cityId);
            return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
        }

        public async Task<Result> ValidateRegionAsync(int regionId)
        {
            var result = await GetRegionAsync(regionId);
            return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
        }

        public async Task<Result> ValidateEventAsync(Guid eventId)
        {
            var result = await GetEventAsync(eventId);
            return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
        }

        public async Task<Result> ValidateUserHasNoProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure("Пользователь не найден");

            if (user.MusicianProfile != null)
                return Result.Failure("Профиль уже существует");

            return Result.Success();
        }

        public async Task<Result<User>> GetUserWithProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<User>.Failure("Пользователь не найден");

            if (user.MusicianProfile == null)
                return Result<User>.Failure("Профиль музыканта не найден");

            return Result<User>.Success(user);
        }

        public async Task<Result<MusicianProfile>> GetMusicianProfileAsync(Guid profileId)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId);
            if (profile == null)
                return Result<MusicianProfile>.Failure("Профиль музыканта не найден");

            return Result<MusicianProfile>.Success(profile);
        }

        public async Task<Result<City>> GetCityAsync(int cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
                return Result<City>.Failure("Город не найден");

            return Result<City>.Success(city);
        }

        public async Task<Result<Region>> GetRegionAsync(int regionId)
        {
            var region = await _regionRepository.GetByIdAsync(regionId);
            if (region == null)
                return Result<Region>.Failure("Регион не найден");

            return Result<Region>.Success(region);
        }

        public async Task<Result<Event>> GetEventAsync(Guid eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null)
                return Result<Event>.Failure("Мероприятие не найдено");

            return Result<Event>.Success(ev);
        }

        public async Task<Result> ValidateGenresExistAsync(List<int>? genreIds)
        {
            if (genreIds == null || genreIds.Count == 0)
                return Result.Success();

            var existingGenres = await _genreRepository.GetByIdsAsync(genreIds);
            if (existingGenres.Count != genreIds.Count)
            {
                var existingIds = existingGenres.Select(g => g.Id).ToHashSet();
                var missingIds = genreIds.Where(id => !existingIds.Contains(id)).ToList();
                return Result.Failure($"Жанры с ID [{string.Join(", ", missingIds)}] не найдены");
            }

            return Result.Success();
        }

        public async Task<Result> ValidateSpecialtiesExistAsync(List<int>? specialtyIds)
        {
            if (specialtyIds == null || specialtyIds.Count == 0)
                return Result.Success();

            var existingSpecialties = await _specialtyRepository.GetByIdsAsync(specialtyIds);
            if (existingSpecialties.Count != specialtyIds.Count)
            {
                var existingIds = existingSpecialties.Select(s => s.Id).ToHashSet();
                var missingIds = specialtyIds.Where(id => !existingIds.Contains(id)).ToList();
                return Result.Failure($"Специальности с ID [{string.Join(", ", missingIds)}] не найдены");
            }

            return Result.Success();
        }

        public async Task<Result> ValidateCollaborationGoalsExistAsync(List<int>? goalIds)
        {
            if (goalIds == null || goalIds.Count == 0)
                return Result.Success();

            var existingGoals = await _goalRepository.GetByIdsAsync(goalIds);
            if (existingGoals.Count != goalIds.Count)
            {
                var existingIds = existingGoals.Select(g => g.Id).ToHashSet();
                var missingIds = goalIds.Where(id => !existingIds.Contains(id)).ToList();
                return Result.Failure($"Цели сотрудничества с ID [{string.Join(", ", missingIds)}] не найдены");
            }

            return Result.Success();
        }
    }
}