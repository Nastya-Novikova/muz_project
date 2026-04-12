using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Media;
using backend.Models.DTOs.Profiles;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql.TypeMapping;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IMusicalSpecialtyRepository _specialtyRepository;
    private readonly ICollaborationGoalRepository _goalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;

    public ProfileService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ICityRepository cityRepository,
        IGenreRepository genreRepository,
        IMusicalSpecialtyRepository specialtyRepository,
        ICollaborationGoalRepository goalRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorage fileStorage)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _cityRepository = cityRepository;
        _genreRepository = genreRepository;
        _specialtyRepository = specialtyRepository;
        _goalRepository = goalRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task<Result<PagedResult<ProfileDto>>> SearchAsync(SearchRequest request)
    {
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
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile == null)
            return Result<ProfileDto>.Failure("Profile not found");

        var dto = _mapper.Map<ProfileDto>(profile);
        return Result<ProfileDto>.Success(dto);
    }

    public async Task<Result<ProfileDto>> GetByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<ProfileDto>.Failure("Profile not found");

        return await GetByIdAsync(user.MusicianProfile.Id);
    }

    public async Task<Result<ProfileDto>> CreateAsync(Guid userId, CreateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<ProfileDto>.Failure("User not found");

        if (user.MusicianProfile != null)
            return Result<ProfileDto>.Failure("Profile already exists");

        var city = await _cityRepository.GetByIdAsync(request.CityId);
        if (city == null)
            return Result<ProfileDto>.Failure("City not found");

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
            profile.Genres = await _genreRepository.GetByIdsAsync(request.GenreIds);

        if (request.SpecialtyIds?.Any() == true)
            profile.Specialties = await _specialtyRepository.GetByIdsAsync(request.SpecialtyIds);

        if (request.CollaborationGoalIds?.Any() == true)
            profile.CollaborationGoals = await _goalRepository.GetByIdsAsync(request.CollaborationGoalIds);

        if (request.DesiredGenreIds?.Any() == true)
            profile.DesiredGenres = await _genreRepository.GetByIdsAsync(request.DesiredGenreIds);

        if (request.DesiredSpecialtyIds?.Any() == true)
            profile.DesiredSpecialties = await _specialtyRepository.GetByIdsAsync(request.DesiredSpecialtyIds);

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
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<ProfileDto>.Failure("Profile not found");

        var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
        if (profile == null)
            return Result<ProfileDto>.Failure("Profile not found");

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
            var city = await _cityRepository.GetByIdAsync(request.CityId.Value);
            if (city == null)
                return Result<ProfileDto>.Failure("City not found");
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
            profile.Genres = await _genreRepository.GetByIdsAsync(request.GenreIds);
        }
        if (request.SpecialtyIds != null)
        {
            profile.Specialties = await _specialtyRepository.GetByIdsAsync(request.SpecialtyIds);
        }
        if (request.CollaborationGoalIds != null)
        {
            profile.CollaborationGoals = await _goalRepository.GetByIdsAsync(request.CollaborationGoalIds);
        }
        if (request.DesiredGenreIds != null)
        {
            profile.DesiredGenres = await _genreRepository.GetByIdsAsync(request.DesiredGenreIds);
        }
        if (request.DesiredSpecialtyIds != null)
        {
            profile.DesiredSpecialties = await _specialtyRepository.GetByIdsAsync(request.DesiredSpecialtyIds);
        }

        profile.UpdatedAt = DateTime.UtcNow;

        await _profileRepository.UpdateAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<ProfileDto>(profile);
        return Result<ProfileDto>.Success(dto);
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<ProfileDto>.Failure("Profile not found");

        var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
        if (profile == null)
            return Result.Failure("Profile not found");

        await _profileRepository.SoftDeleteAsync(profile.Id);
        user.MusicianProfile = null;
        user.ProfileCreated = false;
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<string>> UpdateAvatarAsync(Guid userId, Stream fileStream, string fileName, string contentType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<string>.Failure("Profile not found");

        var profile = await _profileRepository.GetByIdAsync(user.MusicianProfile.Id);
        if (profile == null)
            return Result<string>.Failure("Profile not found");

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
        var profile = await _profileRepository.GetByIdAsync(id);
        if (profile == null)
            return Result<object>.Failure("Profile not found");

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
        var user = await _userRepository.GetByIdAsync(userId);
        if (user?.MusicianProfile == null)
            return Result<NotificationSettingsDto>.Failure("Профиль не найден");

        var settings = new NotificationSettingsDto
        {
            NotifyByEmail = user.MusicianProfile.NotifyByEmail,
            NotifyByVk = user.MusicianProfile.NotifyByVk
        };

        return Result<NotificationSettingsDto>.Success(settings);
    }
}