using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Shared
{
    public abstract class IntegrationTestBase : TestBase, IAsyncLifetime
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(CustomWebApplicationFactory factory, ITestOutputHelper output)
            : base(output)
        {
            Factory = factory;
            Client = Factory.CreateClient();
        }

        public virtual async Task InitializeAsync()
        {
            await Factory.ResetDatabaseAsync();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        protected async Task<(string Token, Guid ProfileId)> CreateUserAndProfileAsync(string email, string fullName)
        {
            await Client.PostAsJsonAsync("/api/auth/code", new { email });
            var loginResponse = await Client.PostAsJsonAsync("/api/auth/session", new { email, code = "111111" });
            loginResponse.EnsureSuccessStatusCode();
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            string token = auth!.Token;

            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Минимальная команда, как в исходных тестах
            var profileCommand = new
            {
                fullName,
                profileType = "Individual",
                cityId = 1
            };

            var profileResponse = await Client.PostAsJsonAsync("/api/profiles", profileCommand);
            profileResponse.EnsureSuccessStatusCode();
            var profileContent = await profileResponse.Content.ReadFromJsonAsync<CreatedProfileResponse>();
            Guid profileId = profileContent!.Id;

            return (token, profileId);
        }

        protected async Task<TestUserData> CreateUserWithProfileDataAsync(string email, string fullName)
        {
            await Client.PostAsJsonAsync("/api/auth/code", new { email });
            var loginResponse = await Client.PostAsJsonAsync("/api/auth/session",
                new { email, code = "111111" });
            loginResponse.EnsureSuccessStatusCode();
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            string token = auth!.Token;
            Guid userId = auth.User.Id;
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Минимальная команда
            var profileCommand = new
            {
                fullName,
                profileType = "Individual",
                cityId = 1
            };

            var profileResponse = await Client.PostAsJsonAsync("/api/profiles", profileCommand);
            profileResponse.EnsureSuccessStatusCode();
            var profileContent = await profileResponse.Content.ReadFromJsonAsync<CreatedProfileResponse>();
            Guid profileId = profileContent!.Id;

            return new TestUserData(token, userId, profileId, email);
        }
    }
}