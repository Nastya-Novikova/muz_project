using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.API.Contracts.Responses;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Extensions;
using MusicianFinder.Tests.Shared.Fixtures;
using MusicianFinder.Tests.Shared.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.E2E
{
    /// <summary>
    /// Сквозные тесты основных пользовательских сценариев.
    /// </summary>
    public class FullUserJourneyTests : TestBase, IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public FullUserJourneyTests(CustomWebApplicationFactory factory, ITestOutputHelper output) : base(output)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Fact]
        public async Task E2E_CompleteUserJourney_Register_Profile_Event_Favorite_Suggestion_Notification_Delete()
        {
            LogInfo("=== E2E: Complete user journey started ===");

            // ---------- Шаг 1: Регистрация двух пользователей и создание профилей ----------
            LogInfo("Step 1: Register users and create profiles");
            var userAlice = await RegisterAndCreateProfileAsync("e2e-alice@test.com", "Alice");
            var userBob = await RegisterAndCreateProfileAsync("e2e-bob@test.com", "Bob");

            LogInfo($"User Alice: Id={userAlice.UserId}, ProfileId={userAlice.ProfileId}");
            LogInfo($"User Bob: Id={userBob.UserId}, ProfileId={userBob.ProfileId}");

            // ---------- Шаг 2: Alice создаёт мероприятие ----------
            LogInfo("Step 2: Alice creates an event");
            var eventStart = DateTime.UtcNow.AddDays(7);
            var createEventCommand = new
            {
                title = "E2E Jazz Night",
                description = "Great jazz concert",
                regionId = 1,
                cityId = 1,
                address = "Test Address",
                startDateTime = eventStart,
                maxParticipants = 2
            };
            SetAuthToken(userAlice.Token);
            var createEventResponse = await _client.PostAsJsonAsync("/api/events", createEventCommand);
            createEventResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<CreatedEventResponse>(_jsonOptions);
            var eventId = createdEvent!.Id;
            LogInfo($"Event created with ID: {eventId}");

            // ---------- Шаг 3: Bob регистрируется на мероприятие ----------
            LogInfo("Step 3: Bob registers for the event");
            SetAuthToken(userBob.Token);
            var registerResponse = await _client.PostAsync($"/api/events/{eventId}/registration", null);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            LogInfo("Bob registered successfully");

            // ---------- Шаг 4: Alice добавляет Bob в избранное ----------
            LogInfo("Step 4: Alice adds Bob to favorites");
            SetAuthToken(userAlice.Token);
            var addFavoriteResponse = await _client.PutAsync($"/api/{userBob.ProfileId}/favorite", null);
            addFavoriteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            LogInfo("Bob added to favorites");

            // ---------- Шаг 5: Bob отправляет предложение о сотрудничестве Alice ----------
            LogInfo("Step 5: Bob sends collaboration suggestion to Alice");
            SetAuthToken(userBob.Token);
            var suggestionCommand = new { toProfileId = userAlice.ProfileId, message = "Let's play together!" };
            var sendSuggestionResponse = await _client.PostAsJsonAsync("/api/suggestions", suggestionCommand);
            sendSuggestionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            LogInfo("Suggestion sent");

            // ---------- Шаг 6: Alice принимает предложение ----------
            LogInfo("Step 6: Alice accepts the suggestion");
            // Сначала получим ID предложения из входящих
            SetAuthToken(userAlice.Token);
            var receivedResponse = await _client.GetAsync("/api/suggestions/received?page=1&limit=10");
            receivedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var receivedPage = await receivedResponse.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>(_jsonOptions);
            var suggestionId = receivedPage!.Items[0].Id;
            var acceptCommand = new { status = "Accepted" };
            var acceptResponse = await _client.PatchJsonAsync($"/api/suggestions/{suggestionId}/status", acceptCommand);
            acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            LogInfo("Suggestion accepted");

            // ---------- Шаг 7: Проверка уведомлений ----------
            LogInfo("Step 7: Check notifications for Alice and Bob");
            // У Alice должно быть уведомление о новом предложении
            SetAuthToken(userAlice.Token);
            var aliceNotifsResponse = await _client.GetAsync("/api/notifications?page=1&limit=10");
            aliceNotifsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var aliceNotifs = await aliceNotifsResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);
            aliceNotifs!.Items.Should().Contain(n => n.Type == "CollaborationReceived" && n.EntityId == suggestionId);
            LogInfo("Alice received collaboration notification");

            // У Bob должно быть уведомление о регистрации на мероприятие
            SetAuthToken(userBob.Token);
            var bobNotifsResponse = await _client.GetAsync("/api/notifications?page=1&limit=10");
            bobNotifsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var bobNotifs = await bobNotifsResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);
            bobNotifs!.Items.Should().Contain(n => n.Type == "EventRegistration" && n.EntityId == eventId);
            LogInfo("Bob received registration notification");

            // ---------- Шаг 8: Проверка флагов на мероприятии ----------
            LogInfo("Step 8: Check event flags for both users");
            SetAuthToken(userAlice.Token);
            var aliceEventResponse = await _client.GetAsync($"/api/events/{eventId}");
            var aliceEvent = await aliceEventResponse.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
            aliceEvent!.IsCreator.Should().BeTrue();
            aliceEvent.IsRegistered.Should().BeFalse();

            SetAuthToken(userBob.Token);
            var bobEventResponse = await _client.GetAsync($"/api/events/{eventId}");
            var bobEvent = await bobEventResponse.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
            bobEvent!.IsCreator.Should().BeFalse();
            bobEvent.IsRegistered.Should().BeTrue();
            LogInfo("Event flags correct");

            // ---------- Шаг 9: Проверка флагов избранного и коллаборации на профилях ----------
            LogInfo("Step 9: Check profile flags");
            SetAuthToken(userAlice.Token);
            var aliceProfileResponse = await _client.GetAsync($"/api/profiles/{userBob.ProfileId}");
            var bobProfileForAlice = await aliceProfileResponse.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);
            bobProfileForAlice!.IsFavorite.Should().BeTrue();
            // NOTE: В текущей реализации IsCollaborated возвращает true только для инициатора предложения.
            // Alice является получателем, поэтому ожидается false. При изменении логики API на симметричную проверку следует обновить.
            bobProfileForAlice.IsCollaborated.Should().BeFalse();

            SetAuthToken(userBob.Token);
            var bobProfileResponse = await _client.GetAsync($"/api/profiles/{userAlice.ProfileId}");
            var aliceProfileForBob = await bobProfileResponse.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);
            aliceProfileForBob!.IsFavorite.Should().BeFalse(); // Bob не добавлял Alice
            aliceProfileForBob.IsCollaborated.Should().BeTrue(); // Bob отправил предложение и оно принято
            LogInfo("Profile flags correct");

            // ---------- Шаг 10: Мягкое удаление профиля Alice ----------
            LogInfo("Step 10: Soft delete Alice profile");
            SetAuthToken(userAlice.Token);
            var deleteResponse = await _client.DeleteAsync("/api/profiles/me");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            LogInfo("Profile deleted");

            // Проверка, что профиль Alice больше не доступен
            var getDeletedProfile = await _client.GetAsync($"/api/profiles/{userAlice.ProfileId}");
            getDeletedProfile.StatusCode.Should().Be(HttpStatusCode.NotFound);
            LogInfo("Deleted profile not accessible");

            // Проверка, что в избранном Bob'а профиль Alice не отображается
            SetAuthToken(userBob.Token);
            var bobFavoritesResponse = await _client.GetAsync("/api/me/favorites?page=1&limit=10");
            bobFavoritesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var bobFavorites = await bobFavoritesResponse.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>(_jsonOptions);
            bobFavorites!.Items.Should().NotContain(p => p.Id == userAlice.ProfileId);
            LogInfo("Deleted profile removed from favorites");

            LogInfo("=== E2E journey completed successfully ===");
        }

        [Fact]
        public async Task E2E_NegativeScenarios_ShouldReturnAppropriateErrors()
        {
            LogInfo("=== E2E: Negative scenarios started ===");

            var userAlice = await RegisterAndCreateProfileAsync("e2e-negative-alice@test.com", "AliceNeg");
            var userBob = await RegisterAndCreateProfileAsync("e2e-negative-bob@test.com", "BobNeg");

            // ---------- 1. Дубликат регистрации на мероприятие ----------
            LogInfo("Negative 1: Duplicate event registration");
            var eventStart = DateTime.UtcNow.AddDays(7);
            var createEventCommand = new
            {
                title = "Test Event",
                regionId = 1,
                cityId = 1,
                address = "Addr",
                startDateTime = eventStart,
                maxParticipants = 5
            };
            SetAuthToken(userAlice.Token);
            var createResponse = await _client.PostAsJsonAsync("/api/events", createEventCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEvent = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>(_jsonOptions);
            var eventId = createdEvent!.Id;

            SetAuthToken(userBob.Token);
            var firstReg = await _client.PostAsync($"/api/events/{eventId}/registration", null);
            firstReg.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var secondReg = await _client.PostAsync($"/api/events/{eventId}/registration", null);
            secondReg.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            LogInfo("Duplicate registration rejected");

            // ---------- 2. Регистрация создателя ----------
            LogInfo("Negative 2: Creator tries to register");
            SetAuthToken(userAlice.Token);
            var creatorReg = await _client.PostAsync($"/api/events/{eventId}/registration", null);
            creatorReg.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await creatorReg.Content.ReadFromJsonAsync<ProblemDetails>(_jsonOptions);
            problem!.Title.Should().Contain("Создатель мероприятия не может зарегистрироваться");
            LogInfo("Creator registration rejected");

            // ---------- 3. Обновление чужого мероприятия ----------
            LogInfo("Negative 3: Update another user's event");
            SetAuthToken(userBob.Token);
            var updateCommand = new { title = "Hacked Title" };
            var updateResponse = await _client.PatchJsonAsync($"/api/events/{eventId}", updateCommand);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            LogInfo("Update forbidden");

            // ---------- 4. Принятие предложения не получателем ----------
            LogInfo("Negative 4: Accept suggestion as sender");
            SetAuthToken(userBob.Token);
            var suggestionCommand = new { toProfileId = userAlice.ProfileId, message = "Hi" };
            await _client.PostAsJsonAsync("/api/suggestions", suggestionCommand);
            // Получим ID предложения от лица отправителя (Bob)
            var sentResponse = await _client.GetAsync("/api/suggestions/sent?page=1&limit=10");
            var sentPage = await sentResponse.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>(_jsonOptions);
            var suggestionId = sentPage!.Items[0].Id;
            var acceptCommand = new { status = "Accepted" };
            var acceptResponse = await _client.PatchJsonAsync($"/api/suggestions/{suggestionId}/status", acceptCommand);
            acceptResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            LogInfo("Accept suggestion by sender forbidden");

            // ---------- 5. Попытка получить непрочитанные уведомления без авторизации ----------
            LogInfo("Negative 5: Unauthorized access to notifications");
            _client.DefaultRequestHeaders.Authorization = null;
            var unauthResponse = await _client.GetAsync("/api/notifications/unread-count");
            unauthResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            LogInfo("Unauthorized access rejected");

            LogInfo("=== Negative scenarios completed ===");
        }

        [Fact]
        public async Task E2E_CanceledEvent_CannotRegister()
        {
            LogInfo("=== E2E: Canceled event cannot register started ===");

            // Arrange: два пользователя
            var userAlice = await RegisterAndCreateProfileAsync("e2e-cancel-alice@test.com", "AliceCancel");
            var userBob = await RegisterAndCreateProfileAsync("e2e-cancel-bob@test.com", "BobCancel");

            // Alice создаёт мероприятие
            var eventStart = DateTime.UtcNow.AddDays(7);
            var createEventCommand = new
            {
                title = "Event to Cancel",
                regionId = 1,
                cityId = 1,
                address = "Somewhere",
                startDateTime = eventStart,
                maxParticipants = 5
            };
            SetAuthToken(userAlice.Token);
            var createResponse = await _client.PostAsJsonAsync("/api/events", createEventCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdEvent = await createResponse.Content.ReadFromJsonAsync<CreatedEventResponse>(_jsonOptions);
            var eventId = createdEvent!.Id;
            LogInfo($"Event created: {eventId}");

            // Alice отменяет мероприятие (DELETE /api/events/{id})
            LogInfo("Alice cancels the event");
            var cancelResponse = await _client.DeleteAsync($"/api/events/{eventId}");
            cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "event cancellation should succeed");

            // Bob пытается зарегистрироваться
            SetAuthToken(userBob.Token);
            var registerResponse = await _client.PostAsync($"/api/events/{eventId}/registration", null);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "registration on cancelled event must be rejected");

            // Проверяем сообщение об ошибке (содержит подстроку "отмен")
            var problem = await registerResponse.Content.ReadFromJsonAsync<ProblemDetails>(_jsonOptions);
            problem!.Title.Should().Contain("отмен", "error should mention cancellation");

            LogInfo("=== Canceled event cannot register test passed ===");
        }

        // ========== Вспомогательные методы ==========

        private async Task<(Guid UserId, Guid ProfileId, string Token)> RegisterAndCreateProfileAsync(string email, string fullName)
        {
            // Запрос кода
            await _client.PostAsJsonAsync("/api/auth/code", new { email });
            // Логин
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/session", new { email, code = "111111" });
            loginResponse.EnsureSuccessStatusCode();
            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
            var token = auth!.Token;

            // Создание профиля
            SetAuthToken(token);
            var profileCommand = new
            {
                fullName = fullName,
                profileType = "Individual",
                cityId = 1,
                experience = 0,
                lookingFor = "NotLooking"
            };
            var profileResponse = await _client.PostAsJsonAsync("/api/profiles", profileCommand);
            profileResponse.EnsureSuccessStatusCode();
            var profileContent = await profileResponse.Content.ReadFromJsonAsync<CreatedProfileResponse>(_jsonOptions);
            Guid profileId = profileContent!.Id;

            return (auth.User.Id, profileId, token);
        }

        private void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}