using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MusicianFinder.Infrastructure.Services;
using Xunit;

namespace MusicianFinder.Tests.Integration.Services
{
    public class RedisCacheServiceTests
    {
        private readonly RedisCacheService _service;
        private readonly IDistributedCache _cache;

        public RedisCacheServiceTests()
        {
            _cache = new MemoryDistributedCache(
                Options.Create(new MemoryDistributedCacheOptions()));
            _service = new RedisCacheService(_cache);
        }

        [Fact]
        public async Task SetAndGet_ReturnsSameData()
        {
            await _service.SetAsync("key", "value", TimeSpan.FromMinutes(5));
            var result = await _service.GetAsync<string>("key");
            result.Should().Be("value");
        }

        [Fact]
        public async Task Get_NonExistent_ReturnsNull()
        {
            var result = await _service.GetAsync<string>("nonexistent");
            result.Should().BeNull();
        }

        [Fact]
        public async Task Remove_ExistingKey_Removes()
        {
            await _service.SetAsync("key", "value", TimeSpan.FromMinutes(5));
            await _service.RemoveAsync("key");
            var result = await _service.GetAsync<string>("key");
            result.Should().BeNull();
        }
    }
}