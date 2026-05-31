using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Extensions;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    public class SuggestionsControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        public SuggestionsControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
            : base(factory, output) { }

        [Fact]
        public async Task SendSuggestion_Valid_Returns204()
        {
            LogInfo("Test: Send suggestion returns 204");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-send@test.com", "Sender");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-recv@test.com", "Receiver");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            var command = new { toProfileId = receiverId, message = "Hello" };
            var response = await Client.PostAsJsonAsync("/api/suggestions", command);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task SendSuggestion_ToNonExistentProfile_Returns404()
        {
            LogInfo("Test: Send suggestion to non-existent profile returns 404");
            var (token, _) = await CreateUserAndProfileAsync("sugg-bad@test.com", "Bad");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var command = new { toProfileId = Guid.NewGuid(), message = "Hello" };
            var response = await Client.PostAsJsonAsync("/api/suggestions", command);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetReceivedSuggestions_ReturnsList()
        {
            LogInfo("Test: Get received suggestions returns list");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-rec1@test.com", "Sender1");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-rec2@test.com", "Receiver1");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = receiverId, message = "Msg" });

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", receiverToken);
            var response = await Client.GetAsync("/api/suggestions/received?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            paged!.Total.Should().Be(1);
            paged.Items[0].FromProfile.Id.ToString().Should().Be(senderId.ToString());
        }

        [Fact]
        public async Task GetSentSuggestions_ReturnsList()
        {
            LogInfo("Test: Get sent suggestions returns list");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-sent1@test.com", "Sender2");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-sent2@test.com", "Receiver2");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = receiverId, message = "Msg" });

            var response = await Client.GetAsync("/api/suggestions/sent?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            paged!.Total.Should().Be(1);
            paged.Items[0].ToProfile.Id.ToString().Should().Be(receiverId.ToString());
        }

        [Fact]
        public async Task AcceptSuggestion_ByReceiver_Returns204()
        {
            LogInfo("Test: Accept suggestion by receiver returns 204");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-accept1@test.com", "Sender3");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-accept2@test.com", "Receiver3");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = receiverId, message = "Hi" });

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", receiverToken);
            var received = await Client.GetAsync("/api/suggestions/received?page=1&limit=10");
            var recvPage = await received.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            var suggestionId = recvPage!.Items[0].Id;

            var acceptCommand = new { status = "Accepted" };
            var acceptResponse = await Client.PatchJsonAsync($"/api/suggestions/{suggestionId}/status", acceptCommand);
            acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var checkSent = await Client.GetAsync("/api/suggestions/received");
            var updated = await checkSent.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            updated!.Items[0].Status.Should().Be("Accepted");
        }

        [Fact]
        public async Task AcceptSuggestion_BySender_Returns403()
        {
            LogInfo("Test: Accept suggestion by sender returns 403");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-forbid1@test.com", "Sender4");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-forbid2@test.com", "Receiver4");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = receiverId, message = "Hi" });

            var sent = await Client.GetAsync("/api/suggestions/sent?page=1&limit=10");
            var sentPage = await sent.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            var suggestionId = sentPage!.Items[0].Id;

            var acceptCommand = new { status = "Accepted" };
            var acceptResponse = await Client.PatchJsonAsync($"/api/suggestions/{suggestionId}/status", acceptCommand);
            acceptResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RejectSuggestion_ByReceiver_Returns204()
        {
            LogInfo("Test: Reject suggestion by receiver returns 204");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-reject1@test.com", "Sender5");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-reject2@test.com", "Receiver5");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = receiverId, message = "Hi" });

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", receiverToken);
            var received = await Client.GetAsync("/api/suggestions/received?page=1&limit=10");
            var recvPage = await received.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            var suggestionId = recvPage!.Items[0].Id;

            var rejectCommand = new { status = "Rejected" };
            var rejectResponse = await Client.PatchJsonAsync($"/api/suggestions/{suggestionId}/status", rejectCommand);
            rejectResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task SendSuggestion_ToSelf_ReturnsNoContent()
        {
            LogInfo("Test: Send suggestion to self returns 204 (not restricted in production)");
            var (token, profileId) = await CreateUserAndProfileAsync("sugg-self@test.com", "Self");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new { toProfileId = profileId, message = "Self" };
            var response = await Client.PostAsJsonAsync("/api/suggestions", command);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateSuggestionStatus_WithInvalidStatus_ReturnsBadRequest()
        {
            LogInfo("Test: Update suggestion with invalid status returns 400");
            var (senderToken, senderId) = await CreateUserAndProfileAsync("sugg-invstat-sender@test.com", "SenderInv");
            var (receiverToken, receiverId) = await CreateUserAndProfileAsync("sugg-invstat-rec@test.com", "ReceiverInv");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", senderToken);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = receiverId, message = "Test" });

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", receiverToken);
            var received = await Client.GetAsync("/api/suggestions/received?page=1&limit=1");
            var page = await received.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>();
            var suggestionId = page!.Items[0].Id;

            var command = new { status = "Withdrawn" };
            var response = await Client.PatchJsonAsync($"/api/suggestions/{suggestionId}/status", command);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}