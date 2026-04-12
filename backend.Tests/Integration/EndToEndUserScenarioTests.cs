using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.Profiles;
using backend.Models.DTOs.Events;
using backend.Models.DTOs.Collaborations;
using backend.Models.DTOs.Notifications;
using backend.Models.DTOs.Common;
using backend.Models.DTOs.Favorites;
using backend.Models.Enums;
using backend.Tests.Helpers;

namespace backend.Tests.Integration;

public class EndToEndUserScenarioTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly JsonSerializerOptions _jsonOptions;

    public EndToEndUserScenarioTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };
    }

    private async Task<string> LoginAsync(string email, string code = "111111")
    {
        var requestCodeResponse = await _client.PostAsJsonAsync("/api/Auth/request-code", new { email });
        requestCodeResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new { email, code });
        loginResponse.EnsureSuccessStatusCode();

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
        return authResponse!.Token;
    }

    private void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private void ClearAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task FullUserJourney_WorksCorrectly()
    {
        var summary = new StringBuilder();
        summary.AppendLine("=== FULL USER JOURNEY TEST SUMMARY ===");
        summary.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        summary.AppendLine();

        await _factory.ResetDatabaseAsync();

        // ========== Пользователь 1 (Alice) ==========
        string aliceEmail = "alice@example.com";
        string aliceToken = await LoginAsync(aliceEmail);
        SetAuthHeader(aliceToken);

        var createProfileRequest = new CreateProfileRequest
        {
            ProfileType = ProfileType.Individual,
            FullName = "Alice Johnson",
            CityId = 1,
            Experience = 5,
            LookingFor = LookingFor.NotLooking,
            GenreIds = new List<int> { 1, 2 },
            SpecialtyIds = new List<int> { 1 },
            CollaborationGoalIds = new List<int> { 1 }
        };

        var profileResponse = await _client.PostAsJsonAsync("/api/Profiles", createProfileRequest);
        profileResponse.EnsureSuccessStatusCode();
        var aliceProfile = await profileResponse.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);

        // NEW: Создаём мероприятие
        var createEventRequest = new CreateEventRequest
        {
            Title = "Alice's Concert",
            Description = "Awesome concert",
            RegionId = 1,
            CityId = 1,
            Address = "Main St, 1",
            StartDateTime = DateTime.UtcNow.AddDays(10),
            MaxParticipants = 50
        };
        var eventResponse = await _client.PostAsJsonAsync("/api/Events", createEventRequest);
        eventResponse.EnsureSuccessStatusCode();
        var createdEvent = await eventResponse.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var eventId = createdEvent!.Id;

        // NEW: Проверяем, что мероприятие создано с 0 участников
        var eventById = await _client.GetAsync($"/api/Events/{eventId}");
        eventById.EnsureSuccessStatusCode();
        var eventAfterCreate = await eventById.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var initialParticipants = eventAfterCreate!.CurrentParticipants;

        ClearAuthHeader();

        // ========== Пользователь 2 (Bob) ==========
        string bobEmail = "bob@example.com";
        string bobToken = await LoginAsync(bobEmail);
        SetAuthHeader(bobToken);

        var bobProfileRequest = new CreateProfileRequest
        {
            ProfileType = ProfileType.Individual,
            FullName = "Bob Smith",
            CityId = 1,
            Experience = 3,
            LookingFor = LookingFor.NotLooking,
            GenreIds = new List<int> { 1 },
            SpecialtyIds = new List<int> { 2 }
        };
        var bobProfileResponse = await _client.PostAsJsonAsync("/api/Profiles", bobProfileRequest);
        bobProfileResponse.EnsureSuccessStatusCode();
        var bobProfile = await bobProfileResponse.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);

        var searchRequest = new SearchRequest { Query = "Alice" };
        var searchResponse = await _client.PostAsJsonAsync("/api/Profiles/search", searchRequest);
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>(_jsonOptions);
        var aliceProfileId = searchResult!.Items.First(p => p.FullName == "Alice Johnson").Id;

        // Регистрируемся на мероприятие
        await _client.PostAsync($"/api/Events/{eventId}/register", null);

        // NEW: Проверяем, что количество участников увеличилось
        var eventAfterReg = await _client.GetAsync($"/api/Events/{eventId}");
        eventAfterReg.EnsureSuccessStatusCode();
        var eventWithReg = await eventAfterReg.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var participantsAfterReg = eventWithReg!.CurrentParticipants;

        // NEW: Проверяем, что Bob зарегистрирован
        var isRegisteredResponse = await _client.GetAsync($"/api/Events/{eventId}");
        var isRegisteredDto = await isRegisteredResponse.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var bobIsRegistered = isRegisteredDto!.IsRegistered;

        // Добавляем Alice в избранное
        await _client.PostAsync($"/api/Favorites/{aliceProfileId}", null);

        // Отправляем предложение о сотрудничестве
        var suggestionRequest = new SendSuggestionRequest { ToProfileId = aliceProfileId, Message = "Let's collaborate!" };
        await _client.PostAsJsonAsync($"/api/Collaborations/{aliceProfileId}", suggestionRequest);

        var favoritesResponse = await _client.GetAsync("/api/Favorites");
        favoritesResponse.EnsureSuccessStatusCode();
        var favorites = await favoritesResponse.Content.ReadFromJsonAsync<PagedResult<FavoriteProfileDto>>(_jsonOptions);

        var sentResponse = await _client.GetAsync("/api/Collaborations/sent");
        sentResponse.EnsureSuccessStatusCode();
        var sentSuggestions = await sentResponse.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>(_jsonOptions);

        ClearAuthHeader();

        // ========== Снова Alice ==========
        SetAuthHeader(aliceToken);

        // NEW: Обновляем мероприятие
        var updateEventRequest = new UpdateEventRequest { Description = "Updated description" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/Events/{eventId}", updateEventRequest);
        updateResponse.EnsureSuccessStatusCode();
        var updatedEvent = await updateResponse.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);

        // Получаем уведомления
        var aliceNotificationsResponse = await _client.GetAsync("/api/Notifications");
        aliceNotificationsResponse.EnsureSuccessStatusCode();
        var aliceNotifications = await aliceNotificationsResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);

        // NEW: Отмечаем первое уведомление как прочитанное
        if (aliceNotifications!.Items.Count > 0)
        {
            var firstNotificationId = aliceNotifications.Items[0].Id;
            await _client.PatchAsync($"/api/Notifications/{firstNotificationId}/read", null);
            // Проверяем, что оно стало прочитанным
            var notificationsAfterRead = await _client.GetAsync("/api/Notifications");
            var notificationsReadDto = await notificationsAfterRead.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);
            var readNotification = notificationsReadDto!.Items.First(n => n.Id == firstNotificationId);
            // Переменная readNotificationIsRead будет использована в проверках
        }

        // NEW: Получаем непрочитанное количество
        var unreadCountResponse = await _client.GetAsync("/api/Notifications/unread-count");
        unreadCountResponse.EnsureSuccessStatusCode();
        var unreadCountJson = await unreadCountResponse.Content.ReadAsStringAsync();
        var unreadCount = JsonSerializer.Deserialize<UnreadCountResponse>(unreadCountJson, _jsonOptions)!.UnreadCount;

        // Получаем полученные предложения
        var receivedResponse = await _client.GetAsync("/api/Collaborations/received");
        receivedResponse.EnsureSuccessStatusCode();
        var receivedSuggestions = await receivedResponse.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>(_jsonOptions);

        // ========== Проверки (Assert) ==========
        var checksPassed = new List<string>();
        var checksFailed = new List<string>();

        void Check(bool condition, string message)
        {
            if (condition) checksPassed.Add(message);
            else checksFailed.Add(message);
        }

        Check(aliceProfile!.FullName == "Alice Johnson", "Alice profile full name is correct");
        Check(bobProfile!.FullName == "Bob Smith", "Bob profile full name is correct");

        Check(createdEvent!.Title == "Alice's Concert", "Event title is correct");
        Check(createdEvent.MaxParticipants == 50, "Event max participants is correct");
        Check(initialParticipants == 0, "Event initially has 0 participants");
        Check(participantsAfterReg == 1, "After Bob registration, participants count is 1");
        Check(bobIsRegistered, "Bob is registered for the event");
        Check(updatedEvent!.Description == "Updated description", "Event updated successfully");

        Check(favorites!.Total == 1, "Bob has 1 favorite");
        if (favorites.Total > 0)
            Check(favorites.Items[0].Profile.FullName == "Alice Johnson", "Bob's favorite is Alice");

        Check(sentSuggestions!.Total == 1, "Bob sent 1 suggestion");
        if (sentSuggestions.Total > 0)
        {
            Check(sentSuggestions.Items[0].ToProfile.Id == aliceProfileId, "Bob's suggestion is to Alice");
            Check(sentSuggestions.Items[0].Message == "Let's collaborate!", "Suggestion message is correct");
        }

        Check(aliceNotifications!.Total == 2, "Alice received 2 notifications");
        if (aliceNotifications.Total >= 2)
        {
            Check(aliceNotifications.Items.Any(n => n.Type == NotificationType.EventRegistration), "Alice has event registration notification");
            Check(aliceNotifications.Items.Any(n => n.Type == NotificationType.CollaborationReceived), "Alice has collaboration received notification");
        }

        // NEW: Проверка отметки прочитанным
        if (aliceNotifications.Items.Count > 0)
        {
            var firstId = aliceNotifications.Items[0].Id;
            var readCheckResponse = await _client.GetAsync("/api/Notifications");
            var readCheckDto = await readCheckResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);
            var isRead = readCheckDto!.Items.First(n => n.Id == firstId).IsRead;
            Check(isRead, "Notification marked as read successfully");
            Check(unreadCount == 1, "Unread count is 1 after marking one notification read");
        }

        Check(receivedSuggestions!.Total == 1, "Alice received 1 suggestion");
        if (receivedSuggestions.Total > 0)
        {
            Check(receivedSuggestions.Items[0].FromProfile.FullName == "Bob Smith", "Suggestion is from Bob");
            Check(receivedSuggestions.Items[0].Message == "Let's collaborate!", "Received message is correct");
        }

        // ========== Формирование итогового вывода ==========
        summary.AppendLine($"CHECKS PASSED: {checksPassed.Count}");
        foreach (var msg in checksPassed)
            summary.AppendLine($"  [OK] {msg}");

        if (checksFailed.Any())
        {
            summary.AppendLine($"CHECKS FAILED: {checksFailed.Count}");
            foreach (var msg in checksFailed)
                summary.AppendLine($"  [FAIL] {msg}");
        }

        summary.AppendLine();
        summary.AppendLine("=== DATA SNAPSHOT ===");
        summary.AppendLine();
        summary.AppendLine("--- Alice Profile ---");
        summary.AppendLine(JsonSerializer.Serialize(aliceProfile, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Bob Profile ---");
        summary.AppendLine(JsonSerializer.Serialize(bobProfile, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Event (after update) ---");
        summary.AppendLine(JsonSerializer.Serialize(updatedEvent, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Bob's Favorites ---");
        summary.AppendLine(JsonSerializer.Serialize(favorites, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Bob's Sent Suggestions ---");
        summary.AppendLine(JsonSerializer.Serialize(sentSuggestions, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Alice's Notifications ---");
        summary.AppendLine(JsonSerializer.Serialize(aliceNotifications, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Alice's Received Suggestions ---");
        summary.AppendLine(JsonSerializer.Serialize(receivedSuggestions, _jsonOptions));

        // Сохранение в файл в папке backend.Tests
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var testsProjectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var filePath = Path.Combine(testsProjectDir, $"test_summary_{timestamp}.txt");
        await File.WriteAllTextAsync(filePath, summary.ToString(), Encoding.UTF8);

        _output.WriteLine($"Test summary saved to: {filePath}");

        if (checksFailed.Any())
        {
            throw new Exception($"Some checks failed: {string.Join(", ", checksFailed)}");
        }
    }

    // Вспомогательный класс для десериализации ответа unread-count
    public class UnreadCountResponse
    {
        public int UnreadCount { get; set; }
    }
}