/*using Microsoft.AspNetCore.Mvc;
using Moq;
using backend.Controllers;
using backend.Services.Interfaces;
using backend.Models.Common;
using backend.Models.DTOs.Auth;
using backend.Tests.Helpers;

namespace backend.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task RequestCode_ValidEmail_ReturnsOk()
    {
        // Arrange
        var request = new RequestCodeRequest { Email = "test@example.com" };
        _authServiceMock.Setup(s => s.RequestCodeAsync(request.Email))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.RequestCode(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("success", okResult.Value!.ToString());
    }

    [Fact]
    public async Task RequestCode_ServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var request = new RequestCodeRequest { Email = "test@example.com" };
        _authServiceMock.Setup(s => s.RequestCodeAsync(request.Email))
            .ReturnsAsync(Result.Failure("Invalid email"));

        // Act
        var result = await _controller.RequestCode(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Invalid email", badResult.Value!.ToString());
    }

    [Fact]
    public async Task Login_ValidCode_ReturnsAuthResponse()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Code = "123456" };
        var expectedResponse = new AuthResponse { Success = true, Token = "jwt" };
        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Code))
            .ReturnsAsync(Result<AuthResponse>.Success(expectedResponse));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Token, actual.Token);
    }

    [Fact]
    public async Task Login_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Code = "wrong" };
        _authServiceMock.Setup(s => s.LoginAsync(request.Email, request.Code))
            .ReturnsAsync(Result<AuthResponse>.Failure("Invalid code"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Invalid code", badResult.Value!.ToString());
    }
}*/