using backend.Models.Common;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Favorites;
using System.Text.Json;

namespace backend.Services.Interfaces;

public interface IFavoriteService
{
    Task<Result> AddFavoriteAsync(Guid userId, Guid profileId);
    Task<Result> RemoveFavoriteAsync(Guid userId, Guid profileId);
    Task<Result<PagedResult<FavoriteProfileDto>>> GetFavoritesAsync(Guid userId, int page, int limit);
    Task<Result<bool>> IsFavoriteAsync(Guid userId, Guid profileId);
}