using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.Features.Auth.DTOs;
using MusicianFinder.Application.Features.Collaborations.DTOs;
using MusicianFinder.Application.Features.Collaborations.SendSuggestion;
using MusicianFinder.Application.Features.Events.CreateEvent;
using MusicianFinder.Application.Features.Events.DTOs;
using MusicianFinder.Application.Features.Events.UpdateEvent;
using MusicianFinder.Application.Features.Favorites.DTOs;
using MusicianFinder.Application.Features.Notifications.DTOs;
using MusicianFinder.Application.Features.Profiles.CreateProfile;
using MusicianFinder.Application.Features.Profiles.DTOs;
using MusicianFinder.Application.Features.Profiles.SearchProfiles;
using MusicianFinder.Domain.Enums;
using MusicianFinder.Tests.Helpers;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.ApiTests.EndToEnd;

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
        if (!requestCodeResponse.IsSuccessStatusCode)
        {
            var errorContent = await requestCodeResponse.Content.ReadAsStringAsync();
            throw new Exception($"❌ Request code failed ({requestCodeResponse.StatusCode}): {errorContent}");
        }

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new { email, code });
        if (!loginResponse.IsSuccessStatusCode)
        {
            var errorContent = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"❌ Login failed ({loginResponse.StatusCode}): {errorContent}");
        }

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

    private string ToQueryString(EventFilterDto filter)
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

    [Fact]
    public async Task FullUserJourney_WorksCorrectly()
    {
        var summary = new StringBuilder();
        summary.AppendLine("=== FULL USER JOURNEY TEST SUMMARY ===");
        summary.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        summary.AppendLine();

        await _factory.ResetDatabaseAsync();

        // Alice
        string aliceEmail = "alice@example.com";
        string aliceToken = await LoginAsync(aliceEmail);
        SetAuthHeader(aliceToken);

        var createProfileRequest = new CreateProfileCommand
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
        if (!profileResponse.IsSuccessStatusCode)
        {
            var error = await GetErrorMessage(profileResponse);
            throw new Exception($"❌ Create profile failed: {error}");
        }
        profileResponse.EnsureSuccessStatusCode();
        var aliceProfile = await profileResponse.Content.ReadFromJsonAsync<ProfileDto>(_jsonOptions);

        var createEventRequest = new CreateEventCommand
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
        var eventId = (await eventResponse.Content.ReadFromJsonAsync<EventIdResponse>(_jsonOptions))!.Id;

        var eventById = await _client.GetAsync($"/api/Events/{eventId}");
        eventById.EnsureSuccessStatusCode();
        var eventAfterCreate = await eventById.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var initialParticipants = eventAfterCreate!.CurrentParticipants;

        ClearAuthHeader();

        // Bob
        string bobEmail = "bob@example.com";
        string bobToken = await LoginAsync(bobEmail);
        SetAuthHeader(bobToken);

        var bobProfileRequest = new CreateProfileCommand
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

        var searchRequest = new SearchProfilesQuery { Query = "Alice" };
        var searchResponse = await _client.PostAsJsonAsync("/api/Profiles/search", searchRequest);
        searchResponse.EnsureSuccessStatusCode();
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<PagedResult<ProfileDto>>(_jsonOptions);
        var aliceProfileId = searchResult!.Items.First(p => p.FullName == "Alice Johnson").Id;

        await _client.PostAsync($"/api/Events/{eventId}/register", null);

        var eventAfterReg = await _client.GetAsync($"/api/Events/{eventId}");
        eventAfterReg.EnsureSuccessStatusCode();
        var eventWithReg = await eventAfterReg.Content.ReadFromJsonAsync<EventDto>(_jsonOptions);
        var participantsAfterReg = eventWithReg!.CurrentParticipants;
        var bobIsRegistered = eventWithReg.IsRegistered;

        await _client.PostAsync($"/api/Favorites/{aliceProfileId}", null);

        var suggestionRequest = new SendSuggestionCommand { ToProfileId = aliceProfileId, Message = "Let's collaborate!" };
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

        // Alice again
        SetAuthHeader(aliceToken);

        var updateEventRequest = new UpdateEventCommand { Description = "Updated description" };
        await _client.PutAsJsonAsync($"/api/Events/{eventId}", updateEventRequest);
        var updatedEvent = await (await _client.GetAsync($"/api/Events/{eventId}")).Content.ReadFromJsonAsync<EventDto>(_jsonOptions);

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

        // Assertions
        aliceProfile!.FullName.Should().Be("Alice Johnson");
        bobProfile!.FullName.Should().Be("Bob Smith");

        eventAfterCreate.Title.Should().Be("Alice's Concert");
        eventAfterCreate.MaxParticipants.Should().Be(50);
        initialParticipants.Should().Be(0);
        participantsAfterReg.Should().Be(1);
        bobIsRegistered.Should().BeTrue();
        updatedEvent!.Description.Should().Be("Updated description");

        favorites!.Total.Should().Be(1);
        favorites.Items[0].Profile.FullName.Should().Be("Alice Johnson");

        sentSuggestions!.Total.Should().Be(1);
        sentSuggestions.Items[0].ToProfile.Id.Should().Be(aliceProfileId);
        sentSuggestions.Items[0].Message.Should().Be("Let's collaborate!");

        bobNotifications!.Total.Should().Be(1);
        bobNotifications.Items[0].Type.Should().Be(NotificationType.EventRegistration);

        aliceNotifications!.Total.Should().Be(1);
        aliceNotifications.Items[0].Type.Should().Be(NotificationType.CollaborationReceived);

        aliceUnreadCount.Should().Be(0);

        receivedSuggestions!.Total.Should().Be(1);
        receivedSuggestions.Items[0].FromProfile.FullName.Should().Be("Bob Smith");
        receivedSuggestions.Items[0].Message.Should().Be("Let's collaborate!");

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var testsProjectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var filePath = Path.Combine(testsProjectDir, $"test_summary_{timestamp}.txt");
        await File.WriteAllTextAsync(filePath, summary.ToString(), Encoding.UTF8);

        _output.WriteLine($"Test summary saved to: {filePath}");
    }

    private class EventIdResponse
    {
        public Guid Id { get; set; }
    }

    private class UnreadCountResponse
    {
        public int UnreadCount { get; set; }
    }

    private async Task<string> GetErrorMessage(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        try
        {
            var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(content, _jsonOptions);
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
}