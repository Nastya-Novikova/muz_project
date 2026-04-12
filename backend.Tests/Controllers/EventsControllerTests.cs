/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Events;
using backend.Models.DTOs.Common;
using backend.Tests.Helpers;

namespace backend.Tests.Controllers;

public class EventsControllerTests : ControllerTestBase
{
    private readonly Mock<IEventService> _eventServiceMock = new();
    private readonly EventsController _controller;

    public EventsControllerTests()
    {
        _controller = new EventsController(_eventServiceMock.Object);
    }

    [Fact]
    public async Task GetEvents_ReturnsPublicEvents()
    {
        // Arrange
        var filter = new EventFilterRequest();
        var paged = new PagedResult<EventDto> { Items = new List<EventDto>(), Total = 0 };
        _eventServiceMock.Setup(s => s.GetEventsAsync(filter))
            .ReturnsAsync(Result<PagedResult<EventDto>>.Success(paged));

        // Act
        var result = await _controller.GetEvents(filter);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task GetEvent_WithAuth_ReturnsEventWithFlag()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        var eventDto = new EventDto { Id = eventId, IsRegistered = true };
        _eventServiceMock.Setup(s => s.GetByIdAsync(eventId, userId))
            .ReturnsAsync(Result<EventDto>.Success(eventDto));

        // Act
        var result = await _controller.GetEvent(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<EventDto>(okResult.Value);
        Assert.True(actual.IsRegistered);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var request = new CreateEventRequest
        {
            Title = "Concert",
            RegionId = 1,
            CityId = 1,
            Address = "Street",
            StartDateTime = DateTime.UtcNow.AddDays(1)
        };
        var created = new EventDto { Id = Guid.NewGuid(), Title = "Concert" };
        _eventServiceMock.Setup(s => s.CreateAsync(userId, request))
            .ReturnsAsync(Result<EventDto>.Success(created));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(created.Id, ((EventDto)okResult.Value!).Id);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsUpdatedEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        var request = new UpdateEventRequest { Title = "Updated" };
        var updated = new EventDto { Id = eventId, Title = "Updated" };
        _eventServiceMock.Setup(s => s.UpdateAsync(userId, eventId, request))
            .ReturnsAsync(Result<EventDto>.Success(updated));

        // Act
        var result = await _controller.Update(eventId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Updated", ((EventDto)okResult.Value!).Title);
    }

    [Fact]
    public async Task Cancel_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        _eventServiceMock.Setup(s => s.CancelAsync(userId, eventId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Cancel(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task Register_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        _eventServiceMock.Setup(s => s.RegisterAsync(userId, eventId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Register(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task Register_EventFull_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        _eventServiceMock.Setup(s => s.RegisterAsync(userId, eventId))
            .ReturnsAsync(Result.Failure("Достигнут лимит участников"));

        // Act
        var result = await _controller.Register(eventId);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Достигнут лимит", badResult.Value!.ToString());
    }

    [Fact]
    public async Task Unregister_Authorized_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        _eventServiceMock.Setup(s => s.UnregisterAsync(userId, eventId))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Unregister(eventId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task UploadImage_ValidImage_ReturnsUrl()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var eventId = Guid.NewGuid();
        var fileMock = CreateMockFormFile("event.jpg", "image/jpeg", new byte[] { 1, 2, 3 });
        _eventServiceMock.Setup(s => s.UploadImageAsync(userId, eventId, It.IsAny<Stream>(), fileMock.Object.FileName, fileMock.Object.ContentType))
            .ReturnsAsync(Result<string>.Success("http://minio/event.jpg"));

        // Act
        var result = await _controller.UploadImage(eventId, fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("imageUrl", okResult.Value!.ToString());
    }

    [Fact]
    public async Task GetMyCreated_Authorized_ReturnsEvents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var paged = new PagedResult<EventDto> { Items = new List<EventDto>(), Total = 0 };
        _eventServiceMock.Setup(s => s.GetMyCreatedEventsAsync(userId, 1, 20))
            .ReturnsAsync(Result<PagedResult<EventDto>>.Success(paged));

        // Act
        var result = await _controller.GetMyCreated(1, 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }

    [Fact]
    public async Task GetMyRegistered_Authorized_ReturnsEvents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var paged = new PagedResult<EventDto> { Items = new List<EventDto>(), Total = 0 };
        _eventServiceMock.Setup(s => s.GetMyRegisteredEventsAsync(userId, 1, 20))
            .ReturnsAsync(Result<PagedResult<EventDto>>.Success(paged));

        // Act
        var result = await _controller.GetMyRegistered(1, 20);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paged, okResult.Value);
    }
}*/