/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Favorites;
using backend.Models.DTOs.Common;
using backend.Tests.Helpers;

namespace backend.Tests.Controllers;

public class FavoritesControllerTests : ControllerTestBase
{
    private readonly Mock<IFavoriteService> _favoriteServiceMock = new();
    private readonly FavoritesController _controller;

    public FavoritesControllerTests()
    {
        _controller = new FavoritesController(_favoriteServiceMock.Object);
    }

    [Fact]
    public async Task GetFavorites_Authorized_ReturnsPaged()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var paged = new PagedResult<FavoriteProfileDto> { Items = new List<FavoriteProfileDto>(), Total = 0 };
        _favoriteServiceMock.Setup(s => s.GetFavoritesAsync(userId, 1, 20))
            .ReturnsAsync(Result<PagedResult<FavoriteProfileDto>>.Success(paged));

        // Act
        var result = await _controller.GetFavorites(1, 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task Add_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        _favoriteServiceMock.Setup(s => s.AddFavoriteAsync(userId, profileId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Add(profileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task Remove_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        _favoriteServiceMock.Setup(s => s.RemoveFavoriteAsync(userId, profileId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Remove(profileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task IsFavorite_ReturnsBool()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        _favoriteServiceMock.Setup(s => s.IsFavoriteAsync(userId, profileId))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.IsFavorite(profileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("isFavorite", okResult.Value!.ToString());
    }
}*/