using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Сервис для работы с файловым хранилищем MinIO.
    /// </summary>
    public class MinioFileStorage : IFileStorage
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;
        private readonly string _publicEndpoint;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MinioFileStorage"/>.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        public MinioFileStorage(IConfiguration configuration)
        {
            var endpoint = configuration["MinIo:Endpoint"];
            var accessKey = configuration["MinIo:AccessKey"];
            var secretKey = configuration["MinIo:SecretKey"];
            _bucketName = configuration["MinIo:BucketName"] ?? "musician-files";
            _publicEndpoint = configuration["MinIo:PublicEndpoint"] ?? endpoint ?? string.Empty;

            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)
                .Build();
        }

        /// <inheritdoc />
        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var objectName = $"{Guid.NewGuid()}_{fileName}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs);

            return $"http://{_publicEndpoint}/{_bucketName}/{objectName}";
        }

        /// <inheritdoc />
        public async Task DeleteFileAsync(string fileUrl)
        {
            var objectName = ExtractObjectNameFromUrl(fileUrl);
            if (string.IsNullOrEmpty(objectName))
                return;

            var removeArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(removeArgs);
        }

        private string? ExtractObjectNameFromUrl(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            if (segments.Length >= 2 && segments[1] == _bucketName)
                return string.Join("/", segments.Skip(2));
            return null;
        }
    }
}