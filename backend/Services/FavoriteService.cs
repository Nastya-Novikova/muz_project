using System.Text.Json;
using AutoMapper;
using backend.Models.Classes;
using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Favorites;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;
using backend.Services.Utils;

namespace backend.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result> AddFavoriteAsync(Guid userId, Guid profileId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result.Failure("User not found");

        var profile = await _profileRepository.GetByIdAsync(profileId);
        if (profile == null)
            return Result.Failure("Profile not found");

        if (await _favoriteRepository.ExistsAsync(userId, profileId))
            return Result.Failure("Already in favorites");

        var favorite = new Favorite { UserId = userId, ProfileId = profileId };
        await _favoriteRepository.AddAsync(favorite);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveFavoriteAsync(Guid userId, Guid profileId)
    {
        if (!await _favoriteRepository.ExistsAsync(userId, profileId))
            return Result.Failure("Not in favorites");

        await _favoriteRepository.RemoveAsync(userId, profileId);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<PagedResult<FavoriteProfileDto>>> GetFavoritesAsync(Guid userId, int page, int limit)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<PagedResult<FavoriteProfileDto>>.Failure("User not found");

        var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(userId, page, limit);
        var total = await _favoriteRepository.CountFavoritesByUserIdAsync(userId);
        var dtos = _mapper.Map<List<FavoriteProfileDto>>(favorites);

        var result = new PagedResult<FavoriteProfileDto>
        {
            Items = dtos,
            Total = total,
            Page = page,
            Limit = limit
        };

        return Result<PagedResult<FavoriteProfileDto>>.Success(result);
    }

    public async Task<Result<bool>> IsFavoriteAsync(Guid userId, Guid profileId)
    {
        var exists = await _favoriteRepository.ExistsAsync(userId, profileId);
        return Result<bool>.Success(exists);
    }

    /*private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public FavoriteService(IUserRepository userRepository, IProfileRepository profileRepository)
    {
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    public async Task<JsonDocument?> AddAsync(Guid userId, Guid favoriteProfileId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.FavoriteProfileIds.Contains(favoriteProfileId)) return null;

            user.FavoriteProfileIds.Add(favoriteProfileId);
            await _userRepository.UpdateAsync(user);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> RemoveAsync(Guid userId, Guid favoriteProfileId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.FavoriteProfileIds.Contains(favoriteProfileId)) return null;

            user.FavoriteProfileIds.Remove(favoriteProfileId);
            await _userRepository.UpdateAsync(user);
            return JsonDocument.Parse(JsonSerializer.Serialize(new { success = true }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<JsonDocument?> GetFavoritesAsync(Guid userId, JsonDocument queryParams)
    {
        try
        {
            var root = queryParams.RootElement;
            var page = root.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
            var limit = root.TryGetProperty("limit", out var l) ? l.GetInt32() : 20;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var allFavoriteIds = user.FavoriteProfileIds;
            var favoriteIds = allFavoriteIds.Skip((page - 1) * limit).Take(limit).ToList();
            var result = await _profileRepository.GetProfilesByIdsAsync(favoriteIds);

            var favorites = result.Select(profile =>
            {
                return new
                {
                    profile.Id,
                    profile.FullName,
                    profile.Description,
                    profile.Phone,
                    profile.Telegram,
                    profile.City,
                    profile.Experience,
                    profile.Age,
                    profile.Avatar,
                    Genres = profile.Genres.Select(g => LookupItemUtil.ToLookupItem(g)),
                    Specialties = profile.Specialties.Select(s => LookupItemUtil.ToLookupItem(s)),
                    CollaborationGoals = profile.CollaborationGoals.Select(g => LookupItemUtil.ToLookupItem(g)),
                };
            });

            return JsonDocument.Parse(JsonSerializer.Serialize(new { favorites }, _options));
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid favoriteUserId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.FavoriteProfileIds.Contains(favoriteUserId) == true;
        }
        catch
        {
            return false;
        }
    }*/
}