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

public class FavoriteService(
    IFavoriteRepository favoriteRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEntityExistenceService existenceService) : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository = favoriteRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IEntityExistenceService _entityExistenceService = existenceService;

    public async Task<Result> AddFavoriteAsync(Guid userId, Guid profileId)
    {
        var userResult = await _entityExistenceService.ValidateUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result.Failure(userResult.Error);

        var profileResult = await _entityExistenceService.ValidateMusicianProfileAsync(profileId);
        if (!profileResult.IsSuccess)
            return Result.Failure(profileResult.Error);

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
        var userResult = await _entityExistenceService.GetUserWithProfileAsync(userId);
        if (!userResult.IsSuccess)
            return Result<PagedResult<FavoriteProfileDto>>.Failure(userResult.Error);
        var user = userResult.Value;

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