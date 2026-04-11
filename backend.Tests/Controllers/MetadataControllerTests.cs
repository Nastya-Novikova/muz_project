/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs;

namespace backend.Tests.Controllers;

public class MetadataControllerTests
{
    private readonly Mock<ICityService> _cityServiceMock = new();
    private readonly Mock<IGenreService> _genreServiceMock = new();
    private readonly Mock<IMusicalSpecialtyService> _specialtyServiceMock = new();
    private readonly Mock<ICollaborationGoalService> _goalServiceMock = new();
    private readonly Mock<IRegionService> _regionServiceMock = new();
    private readonly MetadataController _controller;

    public MetadataControllerTests()
    {
        _controller = new MetadataController(
            _cityServiceMock.Object,
            _genreServiceMock.Object,
            _specialtyServiceMock.Object,
            _goalServiceMock.Object,
            _regionServiceMock.Object);
    }

    [Fact]
    public async Task GetCities_ReturnsList()
    {
        // Arrange
        var cities = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Moscow" } };
        _cityServiceMock.Setup(s => s.GetAllAsync(null, null, false))
            .ReturnsAsync(Result<List<LookupItemDto>>.Success(cities));

        // Act
        var result = await _controller.GetCities();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cities, okResult.Value);
    }

    [Fact]
    public async Task GetRegions_ReturnsList()
    {
        // Arrange
        var regions = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Moscow Oblast" } };
        _regionServiceMock.Setup(s => s.GetAllAsync(null, null, false))
            .ReturnsAsync(Result<List<LookupItemDto>>.Success(regions));

        // Act
        var result = await _controller.GetRegions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(regions, okResult.Value);
    }

    [Fact]
    public async Task GetActivities_ReturnsList()
    {
        // Arrange
        var activities = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Vocalist" } };
        _specialtyServiceMock.Setup(s => s.GetAllAsync(null, null, false))
            .ReturnsAsync(Result<List<LookupItemDto>>.Success(activities));

        // Act
        var result = await _controller.GetActivities();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(activities, okResult.Value);
    }

    [Fact]
    public async Task GetGenres_ReturnsList()
    {
        // Arrange
        var genres = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Rock" } };
        _genreServiceMock.Setup(s => s.GetAllAsync(null, null, false))
            .ReturnsAsync(Result<List<LookupItemDto>>.Success(genres));

        // Act
        var result = await _controller.GetGenres();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(genres, okResult.Value);
    }

    [Fact]
    public async Task GetStatuses_ReturnsList()
    {
        // Arrange
        var statuses = new List<LookupItemDto> { new LookupItemDto { Id = 1, Name = "Band" } };
        _goalServiceMock.Setup(s => s.GetAllAsync(null, null, false))
            .ReturnsAsync(Result<List<LookupItemDto>>.Success(statuses));

        // Act
        var result = await _controller.GetStatuses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(statuses, okResult.Value);
    }
}*/