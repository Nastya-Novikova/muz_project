using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using backend.Models.Classes;
using backend.Models.Repositories.Interfaces;

namespace backend.Tests.Helpers;

public abstract class ControllerTestBase
{
    protected Mock<IUserRepository> UserRepositoryMock { get; } = new();

    protected void SetupUserContext(ControllerBase controller, Guid userId, string email = "test@example.com", string role = "User")
    {
        var claims = new List<Claim>
        {
            new Claim("userId", userId.ToString()),
            new Claim("email", email),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    protected void SetupUserContextWithProfile(ControllerBase controller, Guid userId, Guid profileId)
    {
        SetupUserContext(controller, userId);
        var identity = (ClaimsIdentity)controller.User!.Identity!;
        identity.AddClaim(new Claim("profileId", profileId.ToString()));
    }

    protected Mock<IFormFile> CreateMockFormFile(string fileName, string contentType, byte[] content)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(content.Length);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(content));
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => new MemoryStream(content).CopyToAsync(stream, token))
            .Returns(Task.CompletedTask);
        return fileMock;
    }
}