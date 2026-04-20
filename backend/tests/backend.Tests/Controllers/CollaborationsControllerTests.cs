/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Common;
using backend.Tests.Helpers;

namespace backend.Tests.Controllers;

public class CollaborationsControllerTests : ControllerTestBase
{
    private readonly Mock<ICollaborationService> _collabServiceMock = new();
    private readonly CollaborationsController _controller;

    public CollaborationsControllerTests()
    {
        _controller = new CollaborationsController(_collabServiceMock.Object);
    }

    [Fact]
    public async Task SendSuggestion_Valid_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        var request = new SendSuggestionRequest { ToProfileId = profileId, Message = "Hello" };
        _collabServiceMock.Setup(s => s.SendSuggestionAsync(userId, profileId, request.Message))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.SendSuggestion(profileId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task SendSuggestion_ProfileMismatch_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        var request = new SendSuggestionRequest { ToProfileId = Guid.NewGuid() };

        // Act
        var result = await _controller.SendSuggestion(profileId, request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Profile ID mismatch", badResult.Value!.ToString());
    }

    [Fact]
    public async Task GetReceived_Authorized_ReturnsPagedSuggestions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var paged = new PagedResult<SuggestionDto> { Items = new List<SuggestionDto>(), Total = 0 };
        _collabServiceMock.Setup(s => s.GetReceivedAsync(userId, 1, 20, "createdAt", true))
            .ReturnsAsync(Result<PagedResult<SuggestionDto>>.Success(paged));

        // Act
        var result = await _controller.GetReceived(1, 20, "createdAt", true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task GetSent_Authorized_ReturnsPagedSuggestions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var paged = new PagedResult<SuggestionDto> { Items = new List<SuggestionDto>(), Total = 0 };
        _collabServiceMock.Setup(s => s.GetSentAsync(userId, 1, 20, "createdAt", true))
            .ReturnsAsync(Result<PagedResult<SuggestionDto>>.Success(paged));

        // Act
        var result = await _controller.GetSent(1, 20, "createdAt", true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task IsCollaborated_ReturnsBool()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var profileId = Guid.NewGuid();
        _collabServiceMock.Setup(s => s.IsCollaboratedAsync(userId, profileId))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.IsCollaborated(profileId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("isCollaborated", okResult.Value!.ToString());
    }
}*/