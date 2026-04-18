using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Events;
using backend.Models.DTOs.Media;
using backend.Models.DTOs.Profiles;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql.TypeMapping;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Services;

public class ProfileService(
    IProfileRepository profileRepository,
    IUserRepository userRepository,
    IGenreRepository genreRepository,
    IMusicalSpecialtyRepository specialtyRepository,
    ICollaborationGoalRepository goalRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileStorage fileStorage,
    IValidator<CreateProfileRequest> createValidator,
    IValidator<UpdateProfileRequest> updateValidator,
    IValidator<SearchRequest> searchValidator,
    IEntityExistenceService existenceService) : IProfileService
{
    private readonly IProfileRepository _profileRepository = profileRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IGenreRepository _genreRepository = genreRepository;
    private readonly IMusicalSpecialtyRepository _specialtyRepository = specialtyRepository;
    private readonly ICollaborationGoalRepository _goalRepository = goalRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IFileStorage _fileStorage = fileStorage;
    private readonly IValidator<CreateProfileRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateProfileRequest> _updateValidator = updateValidator;
    private readonly IValidator<SearchRequest> _searchValidator = searchValidator;
    private readonly IEntityExistenceService _existenceService = existenceService;

    public async Task<Result<PagedResult<ProfileDto>>> SearchAsync(SearchRequest request)
    {
        var validationResult = await _searchValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Result<PagedResult<ProfileDto>>.Failure(validationResult.ToErrorString());
        }

        var (items, total) = await _profileRepository.SearchAsync(
            query: request.Query,
            cityId: request.CityId,
            genreIds: request.GenreIds,
            specialtyIds: request.SpecialtyIds,
            goalIds: request.GoalIds,
            desiredGenreIds: request.DesiredGenreIds,
            desiredSpecialtyIds: request.DesiredSpecialtyIds,
            lookingFor: request.LookingFor,
            profileType: request.ProfileType,
            experienceMin: request.ExperienceMin,
            experienceMax: request.ExperienceMax,
            page: request.Page,
            limit: request.Limit,
            sortBy: request.SortBy,
            sortDesc: request.SortDesc);

        var dtos = _mapper.Map<List<ProfileDto>>(items);
        var result = new PagedResult<ProfileDto>
        {
            Items = dtos,
            Total = total,
            Page = request.Page,
            Limit = request.Limit
        };
        return Result<PagedResult<ProfileDto>>.Success(result);
    }

    public async Task<Result<ProfileDto>> GetByIdAsync(Guid id)
    {
        var profileResult = await _existenceService.GetMusicianProfileAsync(id);
        if (!profileResult.IsSuccess)
            return Result<ProfileDto>.Failure(profileResult.Error);
        var profile = profileResult.Value;

        var dto = _mapper.Map<ProfileDto>(profile);
        return Result<ProfileDto>.Success(dto);
    }

    public async Task<Result<ProfileDto>> GetByUserIdAsync(Guid userId)
    {
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<ProfileDto>.Failure(userResult.Error);
        var user = userResult.Value;

        return await GetByIdAsync(user.MusicianProfile.Id);
    }

    public async Task<Result<ProfileDto>> CreateAsync(Guid userId, CreateProfileRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<ProfileDto>.Failure(validationResult.ToErrorString());

        var noProfileCheck = await _existenceService.ValidateUserHasNoProfileAsync(userId);
        if (!noProfileCheck.IsSuccess)
            return Result<ProfileDto>.Failure(noProfileCheck.Error);

        var user = await _userRepository.GetByIdAsync(userId);

        var cityResult = await _existenceService.GetCityAsync(request.CityId);
        if (!cityResult.IsSuccess)
            return Result<ProfileDto>.Failure(cityResult.Error);
        var city = cityResult.Value;

        var profile = new MusicianProfile
        {
            Id = Guid.NewGuid(),
            ProfileType = request.ProfileType,
            FullName = request.FullName,
            Age = request.Age,
            Description = request.Description,
            Phone = request.Phone,
            Telegram = request.Telegram,
            CityId = request.CityId,
            Experience = request.Experience,
            LookingFor = request.LookingFor,
            Email = user.Email,
            NotifyByEmail = true,
            NotifyByVk = false
        };

        if (request.GenreIds?.Any() == true)
        {
            var genresCheck = await _existenceService.ValidateGenresExistAsync(request.GenreIds);
            if (!genresCheck.IsSuccess)
                return Result<ProfileDto>.Failure(genresCheck.Error);
            profile.Genres = await _genreRepository.GetByIdsAsync(request.GenreIds);
        }

        if (request.SpecialtyIds?.Any() == true)
        {
            var specialtiesCheck = await _existenceService.ValidateSpecialtiesExistAsync(request.SpecialtyIds);
            if (!specialtiesCheck.IsSuccess)
                return Result<ProfileDto>.Failure(specialtiesCheck.Error);
            profile.Specialties = await _specialtyRepository.GetByIdsAsync(request.SpecialtyIds);
        }

        if (request.CollaborationGoalIds?.Any() == true)
        {
            var goalsCheck = await _existenceService.ValidateCollaborationGoalsExistAsync(request.CollaborationGoalIds);
            if (!goalsCheck.IsSuccess)
                return Result<ProfileDto>.Failure(goalsCheck.Error);
            profile.CollaborationGoals = await _goalRepository.GetByIdsAsync(request.CollaborationGoalIds);
        }

        if (request.DesiredGenreIds?.Any() == true)
        {
            var desiredGenresCheck = await _existenceService.ValidateGenresExistAsync(request.DesiredGenreIds);
            if (!desiredGenresCheck.IsSuccess)
                return Result<ProfileDto>.Failure(desiredGenresCheck.Error);
            profile.DesiredGenres = await _genreRepository.GetByIdsAsync(request.DesiredGenreIds);
        }

        if (request.DesiredSpecialtyIds?.Any() == true)
        {
            var desiredSpecialtiesCheck = await _existenceService.ValidateSpecialtiesExistAsync(request.DesiredSpecialtyIds);
            if (!desiredSpecialtiesCheck.IsSuccess)
                return Result<ProfileDto>.Failure(desiredSpecialtiesCheck.Error);
            profile.DesiredSpecialties = await _specialtyRepository.GetByIdsAsync(request.DesiredSpecialtyIds);
        }

        await _profileRepository.AddAsync(profile);
        user.MusicianProfile = profile;
        user.ProfileCreated = true;
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<ProfileDto>(profile);
        return Result<ProfileDto>.Success(dto);
    }

    public async Task<Result<ProfileDto>> UpdateAsync(Guid userId, UpdateProfileRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<ProfileDto>.Failure(validationResult.ToErrorString());

        var userValidation = await _existenceService.ValidateUserWithProfileAsync(userId);
        if (!userValidation.IsSuccess)
            return Result<ProfileDto>.Failure(userValidation.Error);

        if (request.CityId.HasValue)
        {
            var cityCheck = await _existenceService.ValidateCityAsync(request.CityId.Value);
            if (!cityCheck.IsSuccess)
                return Result<ProfileDto>.Failure(cityCheck.Error);
        }

        var profile = await _profileRepository.GetByIdAsync((await _userRepository.GetByIdAsync(userId)).MusicianProfile.Id);

        if (request.ProfileType.HasValue)
            profile.ProfileType = request.ProfileType.Value;
        if (!string.IsNullOrWhiteSpace(request.FullName))
            profile.FullName = request.FullName;
        if (request.Age.HasValue)
            profile.Age = request.Age;
        if (request.Description != null)
            profile.Description = request.Description;
        if (request.Phone != null)
            profile.Phone = request.Phone;
        if (request.Telegram != null)
            profile.Telegram = request.Telegram;
        if (request.CityId.HasValue)
        {
            profile.CityId = request.CityId.Value;
        }
        if (request.Experience.HasValue)
            profile.Experience = request.Experience.Value;
        if (request.LookingFor.HasValue)
            profile.LookingFor = request.LookingFor.Value;
        if (request.NotifyByEmail.HasValue)
            profile.NotifyByEmail = request.NotifyByEmail.Value;
        if (request.NotifyByVk.HasValue)
            profile.NotifyByVk = request.NotifyByVk.Value;

        if (request.GenreIds != null)
        {
            var genresCheck = await _existenceService.ValidateGenresExistAsync(request.GenreIds);
            if (!genresCheck.IsSuccess)
                return Result<ProfileDto>.Failure(genresCheck.Error);
        }

        if (request.SpecialtyIds != null)
        {
            var specialtiesCheck = await _existenceService.ValidateSpecialtiesExistAsync(request.SpecialtyIds);
            if (!specialtiesCheck.IsSuccess)
                return Result<ProfileDto>.Failure(specialtiesCheck.Error);
        }

        if (request.CollaborationGoalIds != null)
        {
            var goalsCheck = await _existenceService.ValidateCollaborationGoalsExistAsync(request.CollaborationGoalIds);
            if (!goalsCheck.IsSuccess)
                return Result<ProfileDto>.Failure(goalsCheck.Error);
        }

        if (request.DesiredGenreIds != null)
        {
            var desiredGenresCheck = await _existenceService.ValidateGenresExistAsync(request.DesiredGenreIds);
            if (!desiredGenresCheck.IsSuccess)
                return Result<ProfileDto>.Failure(desiredGenresCheck.Error);
        }

        if (request.DesiredSpecialtyIds != null)
        {
            var desiredSpecialtiesCheck = await _existenceService.ValidateSpecialtiesExistAsync(request.DesiredSpecialtyIds);
            if (!desiredSpecialtiesCheck.IsSuccess)
                return Result<ProfileDto>.Failure(desiredSpecialtiesCheck.Error);
        }

        profile.UpdatedAt = DateTime.UtcNow;

        await _profileRepository.UpdateAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<ProfileDto>(profile);
        return Result<ProfileDto>.Success(dto);
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<ProfileDto>.Failure(userResult.Error);
        var user = userResult.Value;

        await _profileRepository.SoftDeleteAsync(user.MusicianProfile.Id);
        user.MusicianProfile = null;
        user.ProfileCreated = false;
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<string>> UpdateAvatarAsync(Guid userId, Stream fileStream, string fileName, string contentType)
    {
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<string>.Failure(userResult.Error);
        var user = userResult.Value;
        var profile = user.MusicianProfile;

        if (profile.AvatarUrl != null && profile.AvatarUrl != string.Empty)
        {
            await _fileStorage.DeleteFileAsync(profile.AvatarUrl);
        }

        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        profile.AvatarUrl = fileUrl;

        await _profileRepository.UpdateAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        return Result<string>.Success(fileUrl);
    }

    public async Task<Result<object>> GetMediaAsync(Guid id)
    {
        var profileResult = await _existenceService.GetMusicianProfileAsync(id);
        if (!profileResult.IsSuccess)
            return Result<object>.Failure(profileResult.Error);
        var profile = profileResult.Value;

        var media = new
        {
            Audio = _mapper.Map<List<AudioDto>>(profile.AudioFiles),
            Video = _mapper.Map<List<VideoDto>>(profile.VideoFiles),
            Photos = _mapper.Map<List<PhotoDto>>(profile.Photos)
        };

        return Result<object>.Success(media);
    }
    public async Task<Result<NotificationSettingsDto>> GetNotificationSettingsAsync(Guid userId)
    {
        var userResult = await _existenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<NotificationSettingsDto>.Failure(userResult.Error);
        var user = userResult.Value;

        var settings = new NotificationSettingsDto
        {
            NotifyByEmail = user.MusicianProfile.NotifyByEmail,
            NotifyByVk = user.MusicianProfile.NotifyByVk
        };

        return Result<NotificationSettingsDto>.Success(settings);
    }
}