using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    /// <summary>
    /// Интеграционные тесты для <see cref="UserController"/>.
    /// </summary>
    public class UserControllerTests : TestBase, IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output) : base(output)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<(string Token, string UserId, string Email)> CreateUserAsync(string email)
        {
            await _client.PostAsJsonAsync("/api/auth/code", new { email });
            var login = await _client.PostAsJsonAsync("/api/auth/session", new { email, code = "111111" });
            var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
            return (auth!.Token, auth.User.Id.ToString(), auth.User.Email);
        }

        [Fact]
        public async Task GetCurrentUser_Authorized_Returns200()
        {
            LogInfo("Test: Get current user returns 200 with user data");
            var (token, userId, email) = await CreateUserAsync("user-get@test.com");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/api/user");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            user!.Id.ToString().Should().Be(userId);
            user.Email.Should().Be(email);
            user.ProfileCreated.Should().BeFalse();
            user.Role.Should().Be("User");
        }

        [Fact]
        public async Task GetCurrentUser_Unauthorized_Returns401()
        {
            LogInfo("Test: Get current user without auth returns 401");
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/user");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetCurrentUser_AfterProfileCreation_ProfileCreatedTrue()
        {
            LogInfo("Test: Get current user after profile creation shows ProfileCreated=true");
            var (token, userId, email) = await CreateUserAsync("user-profile@test.com");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            // Создаём профиль
            var profileCommand = new { fullName = "Test User", profileType = "Individual", cityId = 1 };
            await _client.PostAsJsonAsync("/api/profiles", profileCommand);
            var response = await _client.GetAsync("/api/user");
            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            user!.ProfileCreated.Should().BeTrue();
        }
    }
}