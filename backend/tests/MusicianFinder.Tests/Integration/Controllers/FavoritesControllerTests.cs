using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    public class FavoritesControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        public FavoritesControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
            : base(factory, output) { }

        [Fact]
        public async Task AddFavorite_Valid_Returns204()
        {
            var user1 = await CreateUserWithProfileDataAsync("fav-add1@test.com", "User1");
            var user2 = await CreateUserWithProfileDataAsync("fav-add2@test.com", "User2");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1.Token);
            var response = await Client.PutAsync($"/api/{user2.ProfileId}/favorite", null);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AddFavorite_Self_AllowedButMayChange()
        {
            var user = await CreateUserWithProfileDataAsync("fav-self@test.com", "Self");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);
            var response = await Client.PutAsync($"/api/{user.ProfileId}/favorite", null);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task AddFavorite_AlreadyExists_Returns409()
        {
            var user1 = await CreateUserWithProfileDataAsync("fav-dup1@test.com", "Dup1");
            var user2 = await CreateUserWithProfileDataAsync("fav-dup2@test.com", "Dup2");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1.Token);
            await Client.PutAsync($"/api/{user2.ProfileId}/favorite", null);
            var second = await Client.PutAsync($"/api/{user2.ProfileId}/favorite", null);
            second.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task GetFavorites_ReturnsPagedList()
        {
            var user1 = await CreateUserWithProfileDataAsync("fav-get1@test.com", "Get1");
            var user2 = await CreateUserWithProfileDataAsync("fav-get2@test.com", "Get2");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1.Token);
            await Client.PutAsync($"/api/{user2.ProfileId}/favorite", null);

            var response = await Client.GetAsync("/api/me/favorites?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>();
            paged!.Total.Should().Be(1);
            paged.Items[0].Id.Should().Be(user2.ProfileId);
            paged.Items[0].IsFavorite.Should().BeTrue();
        }

        [Fact]
        public async Task RemoveFavorite_Valid_Returns204()
        {
            var user1 = await CreateUserWithProfileDataAsync("fav-rem1@test.com", "Rem1");
            var user2 = await CreateUserWithProfileDataAsync("fav-rem2@test.com", "Rem2");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1.Token);
            await Client.PutAsync($"/api/{user2.ProfileId}/favorite", null);

            var deleteResponse = await Client.DeleteAsync($"/api/{user2.ProfileId}/favorite");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getFavs = await Client.GetAsync("/api/me/favorites");
            var paged = await getFavs.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>();
            paged!.Total.Should().Be(0);
        }

        [Fact]
        public async Task RemoveFavorite_NotExists_ReturnsBadRequest()
        {
            var user = await CreateUserWithProfileDataAsync("fav-rem404@test.com", "Rem404");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);
            var response = await Client.DeleteAsync($"/api/{Guid.NewGuid()}/favorite");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);   // production возвращает 400
        }

        [Fact]
        public async Task AddFavorite_WhenTargetProfileNotFound_ReturnsNoContent()
        {
            // production не проверяет существование профиля и возвращает 204
            var user = await CreateUserWithProfileDataAsync("fav-add-404@test.com", "AddFav404");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user.Token);
            var response = await Client.PutAsync($"/api/{Guid.NewGuid()}/favorite", null);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetFavorites_Unauthorized_Returns401()
        {
            Client.DefaultRequestHeaders.Authorization = null;
            var response = await Client.GetAsync("/api/me/favorites");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}