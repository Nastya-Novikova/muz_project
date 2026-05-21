using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Tests.Shared;
using MusicianFinder.Tests.Shared.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Controllers
{
    public class ReferenceDataControllerTests : TestBase, IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ReferenceDataControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output) : base(output)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetCities_Returns200_WithExpectedData()
        {
            LogInfo("Test: Get cities returns expected list");
            var response = await _client.GetAsync("/api/metadata/cities");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cities = await response.Content.ReadFromJsonAsync<List<LookupItemDto>>();
            cities.Should().NotBeEmpty();
            cities.Should().Contain(c => c.Name == "Moscow" && c.LocalizedName == "Москва");
            cities.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetRegions_Returns200()
        {
            LogInfo("Test: Get regions returns 200");
            var response = await _client.GetAsync("/api/metadata/regions");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var regions = await response.Content.ReadFromJsonAsync<List<LookupItemDto>>();
            regions.Should().NotBeEmpty();
            regions.Should().Contain(r => r.Name == "Moscow Oblast");
        }

        [Fact]
        public async Task GetGenres_Returns200()
        {
            LogInfo("Test: Get genres returns 200");
            var response = await _client.GetAsync("/api/metadata/genres");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var genres = await response.Content.ReadFromJsonAsync<List<LookupItemDto>>();
            genres.Should().NotBeEmpty();
            genres.Should().Contain(g => g.Name == "jazz");
        }

        [Fact]
        public async Task GetSpecialties_Returns200()
        {
            LogInfo("Test: Get specialties returns 200");
            var response = await _client.GetAsync("/api/metadata/specialties");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var specialties = await response.Content.ReadFromJsonAsync<List<LookupItemDto>>();
            specialties.Should().NotBeEmpty();
            specialties.Should().Contain(s => s.Name == "vocalist");
        }

        [Fact]
        public async Task GetCollaborationGoals_Returns200()
        {
            LogInfo("Test: Get collaboration goals returns 200");
            var response = await _client.GetAsync("/api/metadata/goals");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var goals = await response.Content.ReadFromJsonAsync<List<LookupItemDto>>();
            goals.Should().NotBeEmpty();
            goals.Should().Contain(g => g.Name == "band");
        }
    }
}