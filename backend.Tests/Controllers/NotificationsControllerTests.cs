/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Notifications;
using backend.Models.DTOs.Common;
using backend.Tests.Helpers;
using backend.Models.Repositories.Interfaces;

namespace backend.Tests.Controllers;

public class NotificationsControllerTests : ControllerTestBase
{
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly NotificationsController _controller;

    public NotificationsControllerTests()
    {
        _controller = new NotificationsController(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task GetNotifications_Authorized_ReturnsPaged()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var paged = new PagedResult<NotificationDto> { Items = new List<NotificationDto>(), Total = 0 };
        _notificationServiceMock.Setup(s => s.GetUserNotificationsAsync(userId, 1, 20))
            .ReturnsAsync(Result<PagedResult<NotificationDto>>.Success(paged));

        // Act
        var result = await _controller.GetNotifications(1, 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task MarkAsRead_Valid_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var notificationId = Guid.NewGuid();
        _notificationServiceMock.Setup(s => s.MarkAsReadAsync(notificationId, userId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.MarkAsRead(notificationId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task MarkAllAsRead_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        _notificationServiceMock.Setup(s => s.MarkAllAsReadAsync(userId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.MarkAllAsRead();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetUnreadCount_Authorized_ReturnsCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        _notificationServiceMock.Setup(s => s.GetUnreadCountAsync(userId))
            .ReturnsAsync(5);

        // Act
        var result = await _controller.GetUnreadCount();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("5", okResult.Value!.ToString());
    }
}*/