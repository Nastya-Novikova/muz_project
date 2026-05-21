using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Notifications;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Extensions;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    public class NotificationsControllerTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        public NotificationsControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
            : base(factory, output) { }

        [Fact]
        public async Task GetNotifications_Authorized_Returns200()
        {
            LogInfo("Test: Get notifications returns 200");
            var (token, _) = await CreateUserAndProfileAsync("notif-get@test.com", "NotifUser");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await Client.GetAsync("/api/notifications?page=1&limit=10");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
            paged.Should().NotBeNull();
        }

        [Fact]
        public async Task GetNotifications_Unauthorized_Returns401()
        {
            LogInfo("Test: Get notifications without auth returns 401");
            Client.DefaultRequestHeaders.Authorization = null;
            var response = await Client.GetAsync("/api/notifications");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUnreadCount_Authorized_Returns200()
        {
            LogInfo("Test: Get unread count returns 200");
            var (token, _) = await CreateUserAndProfileAsync("notif-unread@test.com", "UnreadUser");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await Client.GetAsync("/api/notifications/unread-count");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var count = await response.Content.ReadFromJsonAsync<UnreadCountResponse>();
            count!.UnreadCount.Should().Be(0);
        }

        [Fact]
        public async Task MarkNotificationAsRead_Valid_Returns204()
        {
            LogInfo("Test: Mark notification as read returns 204");
            var (user1Token, profile1Id) = await CreateUserAndProfileAsync("notif-read1@test.com", "Read1");
            var (user2Token, profile2Id) = await CreateUserAndProfileAsync("notif-read2@test.com", "Read2");

            // user2 отправляет предложение user1 -> уведомление создаётся для user1
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user2Token);
            var suggestCommand = new { toProfileId = profile1Id, message = "Test" };
            await Client.PostAsJsonAsync("/api/suggestions", suggestCommand);

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1Token);
            var getNotifs = await Client.GetAsync("/api/notifications?page=1&limit=10");
            var notifs = await getNotifs.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
            var notifId = notifs!.Items[0].Id;

            var markResponse = await Client.PatchAsync($"/api/notifications/{notifId}/read", null);
            markResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAfter = await Client.GetAsync("/api/notifications?page=1&limit=10");
            var afterNotifs = await getAfter.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
            afterNotifs!.Items[0].IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task MarkNotificationAsRead_NonExistent_Returns404()
        {
            LogInfo("Test: Mark non-existent notification returns 404");
            var (token, _) = await CreateUserAndProfileAsync("notif-badid@test.com", "BadId");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await Client.PatchAsync($"/api/notifications/{Guid.NewGuid()}/read", null);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task MarkAllNotificationsAsRead_Returns204()
        {
            LogInfo("Test: Mark all notifications as read returns 204");
            var (user1Token, profile1Id) = await CreateUserAndProfileAsync("notif-all1@test.com", "All1");
            var (user2Token, profile2Id) = await CreateUserAndProfileAsync("notif-all2@test.com", "All2");

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user2Token);
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = profile1Id, message = "Test1" });
            await Client.PostAsJsonAsync("/api/suggestions", new { toProfileId = profile1Id, message = "Test2" });

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1Token);
            var markAllResponse = await Client.PostAsync("/api/notifications/read-all", null);
            markAllResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getNotifs = await Client.GetAsync("/api/notifications?page=1&limit=10");
            var notifs = await getNotifs.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
            notifs!.Items.Should().AllSatisfy(n => n.IsRead.Should().BeTrue());
        }

        [Fact]
        public async Task GetNotificationSettings_Authorized_Returns200()
        {
            LogInfo("Test: Get notification settings returns 200");
            var (token, _) = await CreateUserAndProfileAsync("notif-settings@test.com", "SettingsUser");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await Client.GetAsync("/api/notifications/settings");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var settings = await response.Content.ReadFromJsonAsync<NotificationSettingsResponse>();
            settings!.NotifyByEmail.Should().BeTrue();
            settings.NotifyByVk.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateNotificationSettings_Valid_Returns204()
        {
            LogInfo("Test: Update notification settings returns 204");
            var (token, _) = await CreateUserAndProfileAsync("notif-update@test.com", "UpdateUser");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateCommand = new { notifyByEmail = false, notifyByVk = true };
            var response = await Client.PatchAsJsonAsync("/api/notifications/settings", updateCommand);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync("/api/notifications/settings");
            var settings = await getResponse.Content.ReadFromJsonAsync<NotificationSettingsResponse>();
            settings!.NotifyByEmail.Should().BeFalse();
            settings.NotifyByVk.Should().BeTrue();
        }

        [Fact]
        public async Task MarkNotificationAsRead_WhenNotOwned_ReturnsNotFound()
        {
            LogInfo("Test: Mark notification not owned by current user returns 404");
            var (user1Token, profile1Id) = await CreateUserAndProfileAsync("notif-notowned1@test.com", "NotOwned1");
            var (user2Token, profile2Id) = await CreateUserAndProfileAsync("notif-notowned2@test.com", "NotOwned2");

            // user2 отправляет предложение user1 -> уведомление у user1
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user2Token);
            var suggestCommand = new { toProfileId = profile1Id, message = "Hello" };
            await Client.PostAsJsonAsync("/api/suggestions", suggestCommand);

            // Получаем уведомление как user1
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user1Token);
            var notifResponse = await Client.GetAsync("/api/notifications?page=1&limit=10");
            var paged = await notifResponse.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
            var notifId = paged!.Items[0].Id;

            // Пытаемся отметить уведомление от имени user2 (не владельца)
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", user2Token);
            var markResponse = await Client.PatchAsync($"/api/notifications/{notifId}/read", null);
            markResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}