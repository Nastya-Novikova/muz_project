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
}