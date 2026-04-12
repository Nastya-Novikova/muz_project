/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Uploads;
using backend.Tests.Helpers;
using backend.Models.Repositories.Interfaces;

namespace backend.Tests.Controllers;

public class UploadsControllerTests : ControllerTestBase
{
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly Mock<IAudioUploadService> _audioServiceMock = new();
    private readonly Mock<IVideoUploadService> _videoServiceMock = new();
    private readonly Mock<IPhotoUploadService> _photoServiceMock = new();
    private readonly UploadsController _controller;

    public UploadsControllerTests()
    {
        _controller = new UploadsController(
            _profileServiceMock.Object,
            _audioServiceMock.Object,
            _videoServiceMock.Object,
            _photoServiceMock.Object);
    }

    [Fact]
    public async Task UploadAvatar_ValidImage_ReturnsUrl()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var fileMock = CreateMockFormFile("avatar.jpg", "image/jpeg", new byte[] { 1, 2, 3 });
        _profileServiceMock.Setup(s => s.UpdateAvatarAsync(userId, It.IsAny<Stream>(), fileMock.Object.FileName, fileMock.Object.ContentType))
            .ReturnsAsync(Result<string>.Success("http://minio/avatar.jpg"));

        // Act
        var result = await _controller.UploadAvatar(fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("avatarUrl", okResult.Value!.ToString());
    }

    [Fact]
    public async Task UploadAvatar_InvalidType_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var fileMock = CreateMockFormFile("file.txt", "text/plain", new byte[] { 1 });
        _profileServiceMock.Setup(s => s.UpdateAvatarAsync(userId, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("Only image files are allowed"));

        // Act
        var result = await _controller.UploadAvatar(fileMock.Object);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Only image files are allowed", badResult.Value!.ToString());
    }

    [Fact]
    public async Task UploadAudio_ValidFile_ReturnsUploadResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var fileMock = CreateMockFormFile("song.mp3", "audio/mpeg", new byte[] { 1, 2, 3 });
        var expectedDto = new UploadResultDto { Id = Guid.NewGuid(), Title = "My Song" };
        _audioServiceMock.Setup(s => s.UploadAudioAsync(userId, It.IsAny<Stream>(), fileMock.Object.FileName, fileMock.Object.ContentType, "My Song", "desc"))
            .ReturnsAsync(Result<UploadResultDto>.Success(expectedDto));

        // Act
        var result = await _controller.UploadAudio(fileMock.Object, "My Song", "desc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<UploadResultDto>(okResult.Value);
        Assert.Equal(expectedDto.Id, actual.Id);
    }

    [Fact]
    public async Task UploadVideo_ValidFile_ReturnsUploadResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var fileMock = CreateMockFormFile("video.mp4", "video/mp4", new byte[] { 1, 2, 3 });
        var expectedDto = new UploadResultDto { Id = Guid.NewGuid(), Title = "My Video" };
        _videoServiceMock.Setup(s => s.UploadVideoAsync(userId, It.IsAny<Stream>(), fileMock.Object.FileName, fileMock.Object.ContentType, "My Video", "desc"))
            .ReturnsAsync(Result<UploadResultDto>.Success(expectedDto));

        // Act
        var result = await _controller.UploadVideo(fileMock.Object, "My Video", "desc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<UploadResultDto>(okResult.Value);
        Assert.Equal(expectedDto.Id, actual.Id);
    }

    [Fact]
    public async Task UploadPhoto_ValidFile_ReturnsUploadResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupUserContext(_controller, userId);
        var fileMock = CreateMockFormFile("photo.jpg", "image/jpeg", new byte[] { 1, 2, 3 });
        var expectedDto = new UploadResultDto { Id = Guid.NewGuid(), Title = "My Photo" };
        _photoServiceMock.Setup(s => s.UploadPhotoAsync(userId, It.IsAny<Stream>(), fileMock.Object.FileName, fileMock.Object.ContentType, "My Photo", "desc"))
            .ReturnsAsync(Result<UploadResultDto>.Success(expectedDto));

        // Act
        var result = await _controller.UploadPhoto(fileMock.Object, "My Photo", "desc");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<UploadResultDto>(okResult.Value);
        Assert.Equal(expectedDto.Id, actual.Id);
    }
}*/