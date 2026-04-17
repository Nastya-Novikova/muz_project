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

    private string ToQueryString(EventFilterRequest filter)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(filter.Query))
            queryParams.Add($"Query={Uri.EscapeDataString(filter.Query)}");
        if (filter.RegionId.HasValue)
            queryParams.Add($"RegionId={filter.RegionId}");
        if (filter.CityId.HasValue)
            queryParams.Add($"CityId={filter.CityId}");
        if (filter.FromDate.HasValue)
            queryParams.Add($"FromDate={Uri.EscapeDataString(filter.FromDate.Value.ToString("O"))}");
        if (filter.ToDate.HasValue)
            queryParams.Add($"ToDate={Uri.EscapeDataString(filter.ToDate.Value.ToString("O"))}");
        if (filter.Status.HasValue)
            queryParams.Add($"Status={filter.Status}");
        if (filter.CreatorProfileId.HasValue)
            queryParams.Add($"CreatorProfileId={filter.CreatorProfileId}");
        queryParams.Add($"Page={filter.Page}");
        queryParams.Add($"Limit={filter.Limit}");
        if (!string.IsNullOrEmpty(filter.SortBy))
            queryParams.Add($"SortBy={Uri.EscapeDataString(filter.SortBy)}");
        queryParams.Add($"SortDesc={filter.SortDesc.ToString().ToLowerInvariant()}");
        return string.Join("&", queryParams);
    }

    private async Task<string> GetErrorMessage(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var problem = JsonSerializer.Deserialize<ProblemDetails>(content, _jsonOptions);
            // Собираем все сообщения из Errors, если они есть (валидация атрибутов)
            if (problem?.Errors != null && problem.Errors.Any())
            {
                var allMessages = problem.Errors.SelectMany(kvp => kvp.Value).ToList();
                return string.Join("; ", allMessages);
            }
            return problem?.Detail ?? problem?.Title ?? content;
        }
        catch
        {
            return content;
        }
    }

    private class ProblemDetails
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }

    // ========== ПОЛОЖИТЕЛЬНЫЙ СЦЕНАРИЙ ==========

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

        // Создаём мероприятие
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

        await _client.PostAsync($"/api/Events/{eventId}/register", null);

        var eventAfterReg = await _client.GetAsync($"/api/Events/{eventId}");
        eventAfterReg.EnsureSuccessStatusCode();
        var eventWithReg = await eventAfterReg.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var participantsAfterReg = eventWithReg!.CurrentParticipants;

        var isRegisteredDto = eventWithReg;
        var bobIsRegistered = isRegisteredDto!.IsRegistered;

        await _client.PostAsync($"/api/Favorites/{aliceProfileId}", null);

        var suggestionRequest = new SendSuggestionRequest { ToProfileId = aliceProfileId, Message = "Let's collaborate!" };
        await _client.PostAsJsonAsync($"/api/Collaborations/{aliceProfileId}", suggestionRequest);

        var favoritesResponse = await _client.GetAsync("/api/Favorites");
        favoritesResponse.EnsureSuccessStatusCode();
        var favorites = await favoritesResponse.Content.ReadFromJsonAsync<PagedResult<FavoriteProfileDto>>(_jsonOptions);

        var sentResponse = await _client.GetAsync("/api/Collaborations/sent");
        sentResponse.EnsureSuccessStatusCode();
        var sentSuggestions = await sentResponse.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>(_jsonOptions);

        var bobNotificationsResponse = await _client.GetAsync("/api/Notifications");
        bobNotificationsResponse.EnsureSuccessStatusCode();
        var bobNotifications = await bobNotificationsResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);

        ClearAuthHeader();

        // ========== Снова Alice ==========
        SetAuthHeader(aliceToken);

        var updateEventRequest = new UpdateEventRequest { Description = "Updated description" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/Events/{eventId}", updateEventRequest);
        updateResponse.EnsureSuccessStatusCode();
        var updatedEvent = await updateResponse.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);

        var aliceNotificationsResponse = await _client.GetAsync("/api/Notifications");
        aliceNotificationsResponse.EnsureSuccessStatusCode();
        var aliceNotifications = await aliceNotificationsResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);

        if (aliceNotifications!.Items.Count > 0)
        {
            var firstNotificationId = aliceNotifications.Items[0].Id;
            await _client.PatchAsync($"/api/Notifications/{firstNotificationId}/read", null);
        }

        var aliceUnreadCountResponse = await _client.GetAsync("/api/Notifications/unread-count");
        aliceUnreadCountResponse.EnsureSuccessStatusCode();
        var aliceUnreadCountJson = await aliceUnreadCountResponse.Content.ReadAsStringAsync();
        var aliceUnreadCount = JsonSerializer.Deserialize<UnreadCountResponse>(aliceUnreadCountJson, _jsonOptions)!.UnreadCount;

        var receivedResponse = await _client.GetAsync("/api/Collaborations/received");
        receivedResponse.EnsureSuccessStatusCode();
        var receivedSuggestions = await receivedResponse.Content.ReadFromJsonAsync<PagedResult<SuggestionDto>>(_jsonOptions);

        // ========== Проверки ==========
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

        Check(bobNotifications!.Total == 1, "Bob received 1 notification");
        if (bobNotifications.Total >= 1)
        {
            Check(bobNotifications.Items[0].Type == NotificationType.EventRegistration, "Bob has event registration notification");
        }

        Check(aliceNotifications!.Total == 1, "Alice received 1 notification");
        if (aliceNotifications.Total >= 1)
        {
            Check(aliceNotifications.Items[0].Type == NotificationType.CollaborationReceived, "Alice has collaboration received notification");
        }

        if (aliceNotifications.Items.Count > 0)
        {
            var firstId = aliceNotifications.Items[0].Id;
            var readCheckResponse = await _client.GetAsync("/api/Notifications");
            var readCheckDto = await readCheckResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>(_jsonOptions);
            var isRead = readCheckDto!.Items.First(n => n.Id == firstId).IsRead;
            Check(isRead, "Alice's notification marked as read successfully");
            Check(aliceUnreadCount == 0, "Alice's unread count is 0 after marking her only notification read");
        }

        Check(receivedSuggestions!.Total == 1, "Alice received 1 suggestion");
        if (receivedSuggestions.Total > 0)
        {
            Check(receivedSuggestions.Items[0].FromProfile.FullName == "Bob Smith", "Suggestion is from Bob");
            Check(receivedSuggestions.Items[0].Message == "Let's collaborate!", "Received message is correct");
        }

        // ========== Запись отчёта ==========
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
        summary.AppendLine("--- Bob's Notifications ---");
        summary.AppendLine(JsonSerializer.Serialize(bobNotifications, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Alice's Notifications ---");
        summary.AppendLine(JsonSerializer.Serialize(aliceNotifications, _jsonOptions));
        summary.AppendLine();
        summary.AppendLine("--- Alice's Received Suggestions ---");
        summary.AppendLine(JsonSerializer.Serialize(receivedSuggestions, _jsonOptions));

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

    // ========== ТЕСТЫ ВАЛИДАЦИИ ==========

    [Fact]
    public async Task Validation_RequestCode_InvalidEmail_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new RequestCodeRequest { Email = "not-an-email" };
        var response = await _client.PostAsJsonAsync("/api/Auth/request-code", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("The Email field is not a valid e-mail address", error);
    }

    [Fact]
    public async Task Validation_Login_InvalidEmail_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new LoginRequest { Email = "not-an-email", Code = "111111" };
        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("The Email field is not a valid e-mail address", error);
    }

    [Fact]
    public async Task Validation_Login_InvalidCode_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new LoginRequest { Email = "test@example.com", Code = "123" };
        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Код должен содержать 6 символов", error);
    }

    [Fact]
    public async Task Validation_CreateProfile_EmptyFullName_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("user@example.com");
        SetAuthHeader(token);
        var request = new CreateProfileRequest { FullName = "", CityId = 1 };
        var response = await _client.PostAsJsonAsync("/api/Profiles", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("The FullName field is required", error);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(151)]
    public async Task Validation_CreateProfile_InvalidAge_ReturnsBadRequest(int invalidAge)
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("user@example.com");
        SetAuthHeader(token);
        var request = new CreateProfileRequest { FullName = "Test", CityId = 1, Age = invalidAge };
        var response = await _client.PostAsJsonAsync("/api/Profiles", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        // Для -1 сработает атрибут [Range(0, 2050)]
        if (invalidAge == -1)
            Assert.Contains("The field Age must be between 0 and 2050", error);
        else // 151 сработает FluentValidation
            Assert.Contains("Возраст должен быть от 0 до 150 лет", error);
    }

    [Fact]
    public async Task Validation_CreateProfile_NegativeExperience_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("user@example.com");
        SetAuthHeader(token);
        var request = new CreateProfileRequest { FullName = "Test", CityId = 1, Experience = -5 };
        var response = await _client.PostAsJsonAsync("/api/Profiles", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("The field Experience must be between 0 and", error);
    }

    [Fact]
    public async Task Validation_UpdateProfile_InvalidAge_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("user@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Test", CityId = 1 });
        var request = new UpdateProfileRequest { Age = 200 };
        var response = await _client.PutAsJsonAsync("/api/Profiles", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Возраст должен быть от 0 до 150 лет", error);
    }

    [Fact]
    public async Task Validation_UpdateProfile_NegativeExperience_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("user@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Test", CityId = 1 });
        var request = new UpdateProfileRequest { Experience = -1 };
        var response = await _client.PutAsJsonAsync("/api/Profiles", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("The field Experience must be between 0 and", error);
    }

    [Fact]
    public async Task Validation_CreateEvent_EmptyTitle_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("creator@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Creator", CityId = 1 });
        var request = new CreateEventRequest
        {
            Title = "",
            RegionId = 1,
            CityId = 1,
            Address = "Addr",
            StartDateTime = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 10
        };
        var response = await _client.PostAsJsonAsync("/api/Events", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("The Title field is required", error);
    }

    [Fact]
    public async Task Validation_CreateEvent_PastStartDate_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("creator@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Creator", CityId = 1 });
        var request = new CreateEventRequest
        {
            Title = "Past Event",
            RegionId = 1,
            CityId = 1,
            Address = "Addr",
            StartDateTime = DateTime.UtcNow.AddDays(-1),
            MaxParticipants = 10
        };
        var response = await _client.PostAsJsonAsync("/api/Events", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Дата начала не может быть в прошлом", error);
    }

    [Fact]
    public async Task Validation_CreateEvent_EndBeforeStart_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("creator@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Creator", CityId = 1 });
        var request = new CreateEventRequest
        {
            Title = "Invalid Dates",
            RegionId = 1,
            CityId = 1,
            Address = "Addr",
            StartDateTime = DateTime.UtcNow.AddDays(2),
            EndDateTime = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 10
        };
        var response = await _client.PostAsJsonAsync("/api/Events", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Дата окончания должна быть позже даты начала", error);
    }

    [Fact]
    public async Task Validation_CreateEvent_InvalidMaxParticipants_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("creator@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Creator", CityId = 1 });
        var request = new CreateEventRequest
        {
            Title = "Invalid Capacity",
            RegionId = 1,
            CityId = 1,
            Address = "Addr",
            StartDateTime = DateTime.UtcNow.AddDays(1),
            MaxParticipants = 0
        };
        var response = await _client.PostAsJsonAsync("/api/Events", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Количество участников должно быть от 1 до 1000", error);
    }

    [Fact]
    public async Task Validation_UpdateEvent_PastStartDate_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("creator@example.com");
        SetAuthHeader(token);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Creator", CityId = 1 });
        var createReq = new CreateEventRequest
        {
            Title = "Event",
            RegionId = 1,
            CityId = 1,
            Address = "Addr",
            StartDateTime = DateTime.UtcNow.AddDays(5),
            MaxParticipants = 10
        };
        var createResp = await _client.PostAsJsonAsync("/api/Events", createReq);
        var created = await createResp.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var updateReq = new UpdateEventRequest { StartDateTime = DateTime.UtcNow.AddDays(-1) };
        var response = await _client.PutAsJsonAsync($"/api/Events/{created!.Id}", updateReq);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Дата начала не может быть в прошлом", error);
    }

    [Fact]
    public async Task Validation_SearchProfiles_NegativePage_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new SearchRequest { Page = -1, Limit = 10 };
        var response = await _client.PostAsJsonAsync("/api/Profiles/search", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Номер страницы должен быть >= 1", error);
    }

    [Fact]
    public async Task Validation_SearchProfiles_InvalidLimit_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new SearchRequest { Page = 1, Limit = 0 };
        var response = await _client.PostAsJsonAsync("/api/Profiles/search", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Размер страницы должен быть от 1 до 100", error);
    }

    [Fact]
    public async Task Validation_SearchProfiles_ExperienceMinNegative_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new SearchRequest { ExperienceMin = -1 };
        var response = await _client.PostAsJsonAsync("/api/Profiles/search", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Минимальный опыт не может быть отрицательным", error);
    }

    [Fact]
    public async Task Validation_SearchProfiles_ExperienceMaxLessThanMin_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var request = new SearchRequest { ExperienceMin = 5, ExperienceMax = 2 };
        var response = await _client.PostAsJsonAsync("/api/Profiles/search", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Максимальный опыт должен быть >= минимального", error);
    }

    [Fact]
    public async Task Validation_EventFilter_NegativePage_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var filter = new EventFilterRequest { Page = 0 };
        var response = await _client.GetAsync($"/api/Events?{ToQueryString(filter)}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Номер страницы должен быть >= 1", error);
    }

    [Fact]
    public async Task Validation_EventFilter_InvalidLimit_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var filter = new EventFilterRequest { Limit = 150 };
        var response = await _client.GetAsync($"/api/Events?{ToQueryString(filter)}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Размер страницы должен быть от 1 до 100", error);
    }

    [Fact]
    public async Task Validation_SendSuggestion_EmptyMessage_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string tokenAlice = await LoginAsync("alice@example.com");
        SetAuthHeader(tokenAlice);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Alice", CityId = 1 });
        ClearAuthHeader();
        string tokenBob = await LoginAsync("bob@example.com");
        SetAuthHeader(tokenBob);
        await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "Bob", CityId = 1 });
        var searchResp = await _client.PostAsJsonAsync("/api/Profiles/search", new SearchRequest { Query = "Alice" });
        var searchResult = await searchResp.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>(_jsonOptions);
        var aliceId = searchResult!.Items[0].Id;
        var request = new SendSuggestionRequest { ToProfileId = aliceId, Message = new string('a', 501) };
        var response = await _client.PostAsJsonAsync($"/api/Collaborations/{aliceId}", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("Сообщение не должно превышать 500 символов", error);
    }

    /*[Fact]
    public async Task Validation_SendSuggestion_ToSelf_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        string token = await LoginAsync("user@example.com");
        SetAuthHeader(token);
        var profileResp = await _client.PostAsJsonAsync("/api/Profiles", new CreateProfileRequest { FullName = "User", CityId = 1 });
        var profile = await profileResp.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);
        var request = new SendSuggestionRequest { ToProfileId = profile!.Id, Message = "Self" };
        var response = await _client.PostAsJsonAsync($"/api/Collaborations/{profile.Id}", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await GetErrorMessage(response);
        Assert.Contains("You cannot send a suggestion to yourself", error);
    }*/

    private class UnreadCountResponse
    {
        public int UnreadCount { get; set; }
    }
}