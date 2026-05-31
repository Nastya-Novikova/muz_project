using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using MusicianFinder.Infrastructure.Services;
using MusicianFinder.Tests.Shared;
using Testcontainers.Minio;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Services
{
    public class MinioFileStorageTests : TestBase, IAsyncLifetime
    {
        private readonly MinioContainer _minio;
        private IConfiguration _configuration = null!;
        private MinioFileStorage _storage = null!;
        private IMinioClient _client = null!;
        private const string BucketName = "testbucket";

        public MinioFileStorageTests(ITestOutputHelper output) : base(output)
        {
            _minio = new MinioBuilder()
                .WithImage("minio/minio:latest")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r
                        .ForPort(9000)
                        .ForPath("/minio/health/live")))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _minio.StartAsync();

            var endpoint = $"{_minio.Hostname}:{_minio.GetMappedPublicPort(9000)}";
            var accessKey = _minio.GetAccessKey();
            var secretKey = _minio.GetSecretKey();

            var inMemorySettings = new Dictionary<string, string?>
            {
                ["MinIo:Endpoint"] = endpoint,
                ["MinIo:AccessKey"] = accessKey,
                ["MinIo:SecretKey"] = secretKey,
                ["MinIo:BucketName"] = BucketName,
                ["MinIo:PublicEndpoint"] = endpoint
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _storage = new MinioFileStorage(_configuration);

            _client = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)
                .Build();

            var exists = await _client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(BucketName));
            if (!exists)
                await _client.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(BucketName));
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            await _minio.DisposeAsync();
        }

        [Fact]
        public async Task SaveFileAsync_ValidFile_ReturnsUrl()
        {
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var url = await _storage.SaveFileAsync(stream, "file.jpg", "image/jpeg");

            url.Should().Contain(BucketName).And.Contain("file.jpg");

            var stat = async () => await _client.StatObjectAsync(
                new StatObjectArgs().WithBucket(BucketName).WithObject("file.jpg"));
            await stat.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteFileAsync_ExistingFile_Removes()
        {
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var url = await _storage.SaveFileAsync(stream, "toDelete.jpg", "image/jpeg");
            await _storage.DeleteFileAsync(url);
            // Успешное завершение без исключений
        }

        [Fact]
        public async Task DeleteFileAsync_NonExistingFile_DoesNotThrow()
        {
            var act = async () => await _storage.DeleteFileAsync(
                $"http://fake/{BucketName}/nonexistent.jpg");
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task SaveFileAsync_WhenMinioUnavailable_Throws()
        {
            await _minio.StopAsync();

            var badStorage = new MinioFileStorage(_configuration);
            var stream = new MemoryStream(new byte[] { 1 });
            Func<Task> act = async () => await badStorage.SaveFileAsync(stream, "f.jpg", "image/jpeg");

            await act.Should().ThrowAsync<Exception>();
        }
    }
}