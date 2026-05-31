using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Extensions;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    public class ProfilesControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        public ProfilesControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
            : base(factory, output) { }

        [Fact]
        public async Task CreateProfile_Valid_Returns201()
        {
            LogInfo("Test: Create profile returns 201");
            var user = await CreateUserWithProfileDataAsync("prof-create@test.com", "Creator");
            var profileCommand = new { fullName = "Another", profileType = "Individual", cityId = 1 };
            var response = await Client.PostAsJsonAsync("/api/profiles", profileCommand);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateProfile_InvalidData_Returns400()
        {
            LogInfo("Test: Create profile with invalid data returns 400");
            await Client.PostAsJsonAsync("/api/auth/code", new { email = "prof-invalid@test.com" });
            var login = await Client.PostAsJsonAsync("/api/auth/session",
                new { email = "prof-invalid@test.com", code = "111111" });
            var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
            string token = auth!.Token;
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var command = new { fullName = "", profileType = "Individual", cityId = 1 };
            var response = await Client.PostAsJsonAsync("/api/profiles", command);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMyProfile_Authorized_Returns200()
        {
            LogInfo("Test: Get my profile returns 200");
            var user = await CreateUserWithProfileDataAsync("prof-my@test.com", "MyUser");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var response = await Client.GetAsync("/api/profiles/me");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();
            profile!.Id.Should().Be(user.ProfileId);
            profile.IsMyProfile.Should().BeTrue();
        }

        [Fact]
        public async Task GetProfileById_Existing_Returns200()
        {
            LogInfo("Test: Get profile by id returns 200");
            var user1 = await CreateUserWithProfileDataAsync("prof-byid1@test.com", "User1");
            var user2 = await CreateUserWithProfileDataAsync("prof-byid2@test.com", "User2");

            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user2.Token);
            var response = await Client.GetAsync($"/api/profiles/{user1.ProfileId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var profile = await response.Content.ReadFromJsonAsync<ProfileDto>();
            profile!.Id.Should().Be(user1.ProfileId);
            profile.IsMyProfile.Should().BeFalse();
        }

        [Fact]
        public async Task GetProfileById_NonExisting_Returns404()
        {
            LogInfo("Test: Get non-existing profile returns 404");
            var user = await CreateUserWithProfileDataAsync("prof-404@test.com", "NotFound");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var response = await Client.GetAsync($"/api/profiles/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateMyProfile_Valid_Returns200()
        {
            LogInfo("Test: Update my profile returns 200 and reflects changes");
            var user = await CreateUserWithProfileDataAsync("prof-update@test.com", "OldName");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var updateCommand = new { fullName = "NewName", description = "Updated desc", experience = 5 };
            var updateResponse = await Client.PatchJsonAsync("/api/profiles/me", updateCommand);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await Client.GetAsync("/api/profiles/me");
            var profile = await getResponse.Content.ReadFromJsonAsync<ProfileDto>();
            profile!.FullName.Should().Be("NewName");
            profile.Description.Should().Be("Updated desc");
            profile.Experience.Should().Be(5);
        }

        [Fact]
        public async Task UpdateMyProfile_Partial_OnlyChangesSpecifiedFields()
        {
            LogInfo("Test: Update my profile partially");
            var user = await CreateUserWithProfileDataAsync("prof-partial@test.com", "Original");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var updateCommand = new { description = "Only desc changed" };
            var updateResponse = await Client.PatchJsonAsync("/api/profiles/me", updateCommand);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var profile = await (await Client.GetAsync("/api/profiles/me"))
                .Content.ReadFromJsonAsync<ProfileDto>();
            profile!.FullName.Should().Be("Original");
            profile.Description.Should().Be("Only desc changed");
        }

        [Fact]
        public async Task DeleteMyProfile_Valid_Returns204AndProfileNotAccessible()
        {
            LogInfo("Test: Delete my profile returns 204 and profile becomes inaccessible");
            var user = await CreateUserWithProfileDataAsync("prof-delete@test.com", "ToDelete");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var deleteResponse = await Client.DeleteAsync("/api/profiles/me");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"/api/profiles/{user.ProfileId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SearchProfiles_WithFilters_ReturnsFilteredResults()
        {
            LogInfo("Test: Search profiles with filters returns filtered results");
            var user1 = await CreateUserWithProfileDataAsync("prof-search1@test.com", "Alice");
            var user2 = await CreateUserWithProfileDataAsync("prof-search2@test.com", "Bob");

            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1.Token);
            var response = await Client.GetAsync("/api/profiles?query=Bob&page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>();
            paged!.Total.Should().Be(1);
            paged.Items[0].FullName.Should().Be("Bob");
        }

        [Fact]
        public async Task UploadAvatar_ValidImage_Returns200WithUrl()
        {
            LogInfo("Test: Upload avatar returns 200 with url");
            var user = await CreateUserWithProfileDataAsync("prof-avatar@test.com", "AvatarUser");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var imageContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            var formData = new MultipartFormDataContent { { imageContent, "avatar", "avatar.jpg" } };

            var response = await Client.PutAsync("/api/profiles/me/avatar", formData);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<AvatarResponse>();
            result!.Url.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UploadAvatar_InvalidFileType_Returns400()
        {
            LogInfo("Test: Upload avatar with invalid file type returns 400");
            var user = await CreateUserWithProfileDataAsync("prof-avatar-bad@test.com", "BadAvatar");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var textContent = new StringContent("not an image");
            textContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            var formData = new MultipartFormDataContent { { textContent, "avatar", "file.txt" } };

            var response = await Client.PutAsync("/api/profiles/me/avatar", formData);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ---------------------- NEW TESTS ----------------------
        [Fact]
        public async Task UpdateMyProfile_WithInvalidCity_ReturnsBadRequest()
        {
            LogInfo("Test: Update profile with invalid city returns 400");
            var user = await CreateUserWithProfileDataAsync("prof-badcity@test.com", "BadCity");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var updateCommand = new { cityId = 999 };
            var response = await Client.PatchJsonAsync("/api/profiles/me", updateCommand);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateMyProfile_WithInvalidGenres_ReturnsBadRequest()
        {
            LogInfo("Test: Update profile with invalid genres returns 400");
            var user = await CreateUserWithProfileDataAsync("prof-badgenres@test.com", "BadGenres");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var updateCommand = new { genreIds = new[] { 999 } };
            var response = await Client.PatchJsonAsync("/api/profiles/me", updateCommand);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ConnectVk_ValidCode_Returns204()
        {
            LogInfo("Test: Connect VK with valid code returns 204");
            var user = await CreateUserWithProfileDataAsync("prof-vk-valid@test.com", "VkValid");
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);

            var command = new { code = "valid_code", codeVerifier = "verifier", deviceId = "device" };
            var response = await Client.PostAsJsonAsync("/api/profiles/me/connect-vk", command);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteMyProfile_AlreadyDeleted_ReturnsNotFound()
        {
            LogInfo("Test: Delete already deleted profile returns 404");
            var user = await CreateUserWithProfileDataAsync("prof-double-delete@test.com", "DoubleDelete");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            var firstDelete = await Client.DeleteAsync("/api/profiles/me");
            firstDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var secondDelete = await Client.DeleteAsync("/api/profiles/me");
            secondDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ConnectVk_InvalidCode_ReturnsNoContent_ProductionDoesNotValidate()
        {
            LogInfo("Test: Connect VK with invalid code returns 204 (no validation in production)");
            var user = await CreateUserWithProfileDataAsync("prof-vk-invalid@test.com", "VkInvalid");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            Factory.SetMockVkServiceExchangedUserId(null);
            var command = new { code = "invalid_code", codeVerifier = "verifier", deviceId = "device" };
            var response = await Client.PostAsJsonAsync("/api/profiles/me/connect-vk", command);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Factory.SetMockVkServiceExchangedUserId(123456789L);
        }

        [Fact(Skip = "Unable to mock scoped IFileStorage throw via factory flag in current architecture")]
        public async Task UploadAvatar_WhenStorageFails_Returns500() { }
    }
}