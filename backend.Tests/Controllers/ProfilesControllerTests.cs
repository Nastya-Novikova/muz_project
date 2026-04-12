/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Profiles;
using backend.Models.DTOs.Common;
using backend.Tests.Helpers;

namespace backend.Tests.Controllers;

public class ProfilesControllerTests : ControllerTestBase
{
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly ProfilesController _controller;

    public ProfilesControllerTests()
    {
        _controller = new ProfilesController(_profileServiceMock.Object);
    }

    [Fact]
    public async Task Search_ReturnsPagedProfiles()
    {
        // Arrange
        var request = new SearchRequest();
        var paged = new PagedResult<ProfileDto> { Items = new List<ProfileDto>(), Total = 0 };
        _profileServiceMock.Setup(s => s.SearchAsync(request))
            .ReturnsAsync(Result<PagedResult<ProfileDto>>.Success(paged));

        // Act
        var result = await _controller.Search(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task GetMyProfile_Authorized_ReturnsProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var expectedProfile = new ProfileDto { Id = Guid.NewGuid(), FullName = "Test User" };
        _profileServiceMock.Setup(s => s.GetByUserIdAsync(userId))
            .ReturnsAsync(Result<ProfileDto>.Success(expectedProfile));

        // Act
        var result = await _controller.GetMyProfile();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<ProfileDto>(okResult.Value);
        Assert.Equal(expectedProfile.FullName, actual.FullName);
    }

    [Fact]
    public async Task Get_ReturnsProfile()
    {
        // Arrange
        var profileId = Guid.NewGuid();
        var profile = new ProfileDto { Id = profileId, FullName = "Public User" };
        _profileServiceMock.Setup(s => s.GetByIdAsync(profileId))
            .ReturnsAsync(Result<ProfileDto>.Success(profile));

        // Act
        var result = await _controller.Get(profileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(profile.FullName, ((ProfileDto)okResult.Value!).FullName);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var request = new CreateProfileRequest { FullName = "New", CityId = 1 };
        var created = new ProfileDto { Id = Guid.NewGuid(), FullName = "New" };
        _profileServiceMock.Setup(s => s.CreateAsync(userId, request))
            .ReturnsAsync(Result<ProfileDto>.Success(created));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(created.Id, ((ProfileDto)okResult.Value!).Id);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var request = new UpdateProfileRequest { FullName = "Updated" };
        var updated = new ProfileDto { FullName = "Updated" };
        _profileServiceMock.Setup(s => s.UpdateAsync(userId, request))
            .ReturnsAsync(Result<ProfileDto>.Success(updated));

        // Act
        var result = await _controller.Update(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Updated", ((ProfileDto)okResult.Value!).FullName);
    }

    [Fact]
    public async Task GetMedia_Authorized_ReturnsMedia()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        var media = new { Audio = new List<object>(), Video = new List<object>(), Photos = new List<object>() };
        _profileServiceMock.Setup(s => s.GetMediaAsync(profileId))
            .ReturnsAsync(Result<object>.Success(media));

        // Act
        var result = await _controller.GetMedia(profileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Delete_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        _profileServiceMock.Setup(s => s.DeleteAsync(userId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Delete();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }
}*/