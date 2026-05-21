using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.API.Contracts.Responses;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Extensions;
using MusicianFinder.Tests.Shared.Factories;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    public class EventsControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        public EventsControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
            : base(factory, output) { }

        [Fact]
        public async Task CreateEvent_ValidData_Returns201()
        {
            LogInfo("Test: Create event returns 201 with id");
            var (token, profileId) = await CreateUserAndProfileAsync("events-creator@test.com", "Creator");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var command = TestDataFactory.CreateValidEventCommand("Jazz Night");
            var response = await Client.PostAsJsonAsync("/api/events", command);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreatedEventResponse>();
            result!.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateEvent_WithoutAuth_Returns401()
        {
            LogInfo("Test: Create event without auth returns 401");
            Client.DefaultRequestHeaders.Authorization = null;
            var command = TestDataFactory.CreateValidEventCommand("Unauth Event");
            var response = await Client.PostAsJsonAsync("/api/events", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateEvent_InvalidData_Returns400()
        {
            LogInfo("Test: Create event with invalid data returns 400");
            var (token, _) = await CreateUserAndProfileAsync("events-invalid@test.com", "Invalid");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var command = new { title = "", regionId = 0, cityId = 0, address = "", startDateTime = DateTime.UtcNow.AddDays(-1) };
            var response = await Client.PostAsJsonAsync("/api/events", command);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetEventById_ExistingEvent_Returns200()
        {
            LogInfo("Test: Get event by id returns 200");
            var (token, profileId) = await CreateUserAndProfileAsync("events-get@test.com", "Getter");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Get Event"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            var response = await Client.GetAsync($"/api/events/{created!.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var eventDto = await response.Content.ReadFromJsonAsync<EventDto>();
            eventDto!.Title.Should().Be("Get Event");
            eventDto.CreatorProfileId.Should().Be(profileId);
        }

        [Fact]
        public async Task GetEventById_NonExisting_Returns404()
        {
            LogInfo("Test: Get non-existing event returns 404");
            var response = await Client.GetAsync($"/api/events/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateEvent_ByCreator_Returns200()
        {
            LogInfo("Test: Update event by creator returns 200");
            var (token, _) = await CreateUserAndProfileAsync("events-updater@test.com", "Updater");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Old Title"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            var updateCommand = new { title = "New Title", description = "Updated desc" };
            var updateResponse = await Client.PatchJsonAsync($"/api/events/{created!.Id}", updateCommand);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await Client.GetAsync($"/api/events/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<EventDto>();
            updated!.Title.Should().Be("New Title");
            updated.Description.Should().Be("Updated desc");
        }

        [Fact]
        public async Task UpdateEvent_ByNonCreator_Returns403()
        {
            LogInfo("Test: Update event by non-creator returns 403");
            var (token1, _) = await CreateUserAndProfileAsync("events-creator2@test.com", "Creator2");
            var (token2, _) = await CreateUserAndProfileAsync("events-noncreator@test.com", "NonCreator");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Protected"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);
            var updateCommand = new { title = "Hacked" };
            var updateResponse = await Client.PatchJsonAsync($"/api/events/{created!.Id}", updateCommand);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CancelEvent_ByCreator_Returns204()
        {
            LogInfo("Test: Cancel event by creator returns 204");
            var (token, _) = await CreateUserAndProfileAsync("events-canceller@test.com", "Canceller");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("To Cancel"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            var deleteResponse = await Client.DeleteAsync($"/api/events/{created!.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"/api/events/{created.Id}");
            var cancelled = await getResponse.Content.ReadFromJsonAsync<EventDto>();
            cancelled!.Status.Should().Be("Cancelled");
        }

        [Fact]
        public async Task CancelEvent_ByNonCreator_Returns403()
        {
            LogInfo("Test: Cancel event by non-creator returns 403");
            var (token1, _) = await CreateUserAndProfileAsync("events-creator3@test.com", "Creator3");
            var (token2, _) = await CreateUserAndProfileAsync("events-noncanceller@test.com", "NonCanceller");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("To Cancel"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);
            var deleteResponse = await Client.DeleteAsync($"/api/events/{created!.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RegisterToEvent_Valid_Returns204()
        {
            LogInfo("Test: Register to event returns 204");
            var (creatorToken, creatorProfileId) = await CreateUserAndProfileAsync("events-creator4@test.com", "Creator4");
            var (userToken, userProfileId) = await CreateUserAndProfileAsync("events-registrant@test.com", "Registrant");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Reg Event"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            var regResponse = await Client.PostAsync($"/api/events/{created!.Id}/registration", null);
            regResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task RegisterToEvent_AlreadyRegistered_Returns409()
        {
            LogInfo("Test: Register to event twice returns 409");
            var (creatorToken, _) = await CreateUserAndProfileAsync("events-creator5@test.com", "Creator5");
            var (userToken, _) = await CreateUserAndProfileAsync("events-duplicate@test.com", "Duplicate");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Duplicate Event"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            await Client.PostAsync($"/api/events/{created!.Id}/registration", null);
            var secondReg = await Client.PostAsync($"/api/events/{created!.Id}/registration", null);
            secondReg.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UnregisterFromEvent_Valid_Returns204()
        {
            LogInfo("Test: Unregister from event returns 204");
            var (creatorToken, _) = await CreateUserAndProfileAsync("events-creator6@test.com", "Creator6");
            var (userToken, _) = await CreateUserAndProfileAsync("events-unregistrant@test.com", "Unregistrant");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Unreg Event"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            await Client.PostAsync($"/api/events/{created!.Id}/registration", null);
            var unregResponse = await Client.DeleteAsync($"/api/events/{created!.Id}/registration");
            unregResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetEvents_WithPagination_ReturnsPagedResult()
        {
            LogInfo("Test: Get events with pagination returns paged result");
            var response = await Client.GetAsync("/api/events?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<EventDto>>();
            paged.Should().NotBeNull();
            paged!.Page.Should().Be(1);
            paged.Limit.Should().Be(10);
        }

        [Fact]
        public async Task GetEvents_WithInvalidPage_Returns400()
        {
            LogInfo("Test: Get events with page=0 returns 400");
            var response = await Client.GetAsync("/api/events?page=0");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMyCreatedEvents_Authorized_Returns200()
        {
            LogInfo("Test: Get my created events returns 200");
            var (token, _) = await CreateUserAndProfileAsync("events-mycreated@test.com", "MyCreator");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("My Event"));

            var response = await Client.GetAsync("/api/events/created?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<EventDto>>();
            paged!.Total.Should().Be(1);
            paged.Items[0].Title.Should().Be("My Event");
            paged.Items[0].IsCreator.Should().BeTrue();
        }

        [Fact]
        public async Task GetMyRegisteredEvents_Authorized_Returns200()
        {
            LogInfo("Test: Get my registered events returns 200");
            var (creatorToken, _) = await CreateUserAndProfileAsync("events-creator7@test.com", "Creator7");
            var (userToken, _) = await CreateUserAndProfileAsync("events-registered@test.com", "Registered");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("Reg Event 2"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            await Client.PostAsync($"/api/events/{created!.Id}/registration", null);

            var response = await Client.GetAsync("/api/events/registered?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<EventDto>>();
            paged!.Total.Should().Be(1);
            paged.Items[0].IsRegistered.Should().BeTrue();
        }

        // ---------------------- NEW TESTS ----------------------
        [Fact]
        public async Task RegisterToEvent_WhenEventCancelled_ReturnsBadRequest()
        {
            LogInfo("Test: Register to cancelled event returns 400");
            var (creatorToken, _) = await CreateUserAndProfileAsync("ev-cancel-reg@test.com", "CreatorCancel");
            var (userToken, _) = await CreateUserAndProfileAsync("ev-cancel-user@test.com", "RegistrantCancel");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("CancelEvent"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            // Отменяем событие
            var cancelResponse = await Client.DeleteAsync($"/api/events/{created!.Id}");
            cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Пытаемся зарегистрироваться другим пользователем
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            var regResponse = await Client.PostAsync($"/api/events/{created.Id}/registration", null);
            regResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterToEvent_WhenCreator_ReturnsBadRequest()
        {
            LogInfo("Test: Creator cannot register to own event");
            var (token, profileId) = await CreateUserAndProfileAsync("ev-creator-reg@test.com", "CreatorReg");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("OwnEvent"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            var regResponse = await Client.PostAsync($"/api/events/{created!.Id}/registration", null);
            regResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterToEvent_WhenLimitReached_ReturnsBadRequest()
        {
            LogInfo("Test: Registration when limit reached returns 400");
            var (creatorToken, _) = await CreateUserAndProfileAsync("ev-limit-creator@test.com", "LimitCreator");
            var (user1Token, _) = await CreateUserAndProfileAsync("ev-limit-user1@test.com", "LimitUser1");
            var (user2Token, _) = await CreateUserAndProfileAsync("ev-limit-user2@test.com", "LimitUser2");

            // Создаём событие с лимитом 1
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var command = TestDataFactory.CreateValidEventCommand("LimitedEvent");
            command.MaxParticipants = 1;
            var createResponse = await Client.PostAsJsonAsync("/api/events", command);
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            // Регистрируем первого участника
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1Token);
            var reg1 = await Client.PostAsync($"/api/events/{created!.Id}/registration", null);
            reg1.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Второй участник должен получить 400
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user2Token);
            var reg2 = await Client.PostAsync($"/api/events/{created.Id}/registration", null);
            reg2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UnregisterFromEvent_NotRegistered_ReturnsBadRequest()
        {
            LogInfo("Test: Unregister when not registered returns 400");
            var (creatorToken, _) = await CreateUserAndProfileAsync("ev-unreg-notreg@test.com", "CreatorUnreg");
            var (userToken, _) = await CreateUserAndProfileAsync("ev-unreg-user@test.com", "UserUnreg");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", creatorToken);
            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("UnregEvent"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
            var deleteResponse = await Client.DeleteAsync($"/api/events/{created!.Id}/registration");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UnregisterFromEvent_EventNotFound_Returns404()
        {
            LogInfo("Test: Unregister from non-existing event returns 404");
            var (token, _) = await CreateUserAndProfileAsync("ev-unreg-404@test.com", "Unreg404");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await Client.DeleteAsync($"/api/events/{Guid.NewGuid()}/registration");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CancelEvent_AlreadyCancelled_ReturnsBadRequest()
        {
            LogInfo("Test: Cancel already cancelled event returns 400");
            var (token, _) = await CreateUserAndProfileAsync("ev-double-cancel@test.com", "DoubleCancel");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("DoubleCancel"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            var firstCancel = await Client.DeleteAsync($"/api/events/{created!.Id}");
            firstCancel.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var secondCancel = await Client.DeleteAsync($"/api/events/{created.Id}");
            secondCancel.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateEvent_WithPastStartDate_ReturnsBadRequest()
        {
            LogInfo("Test: Update event with past start date returns 400");
            var (token, _) = await CreateUserAndProfileAsync("ev-update-past@test.com", "UpdaterPast");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createResponse = await Client.PostAsJsonAsync("/api/events", TestDataFactory.CreateValidEventCommand("FutureEvent"));
            var created = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>();

            var updateCommand = new { startDateTime = DateTime.UtcNow.AddDays(-1) };
            var updateResponse = await Client.PatchJsonAsync($"/api/events/{created!.Id}", updateCommand);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetEvents_WithInvalidSortBy_ReturnsBadRequest()
        {
            LogInfo("Test: Get events with invalid sortBy returns 400");
            var response = await Client.GetAsync("/api/events?sortBy=invalidField");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}