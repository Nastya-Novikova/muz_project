using backend.Services.Interfaces;
using Minio.DataModel.Args;
using Minio;

namespace backend.Services
{
    public class MinIoFileStorage : IFileStorage
    {
        private readonly IMinioClient _minIoClient;
        private readonly string _bucketName;
        private readonly string _publicEndpoint;

        public MinIoFileStorage(IConfiguration configuration)
        {
            var endpoint = configuration["MinIo:Endpoint"];
            var accessKey = configuration["MinIo:AccessKey"];
            var secretKey = configuration["MinIo:SecretKey"];
            _bucketName = configuration["MinIo:BucketName"] ?? "musician-files";
            _publicEndpoint = configuration["MinIo:PublicEndpoint"] ?? endpoint ?? string.Empty;

            _minIoClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)
                .Build();
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var objectName = $"{Guid.NewGuid()}_{fileName}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minIoClient.PutObjectAsync(putObjectArgs);

            return $"http://{_publicEndpoint}/{_bucketName}/{objectName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var objectName = ExtractObjectNameFromUrl(fileUrl);
            if (string.IsNullOrEmpty(objectName)) return;

            var removeArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);

            await _minIoClient.RemoveObjectAsync(removeArgs);
        }

        public async Task<bool> FileExistsAsync(string fileUrl)
        {
            var objectName = ExtractObjectNameFromUrl(fileUrl);
            if (string.IsNullOrEmpty(objectName)) return false;

            try
            {
                var statArgs = new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName);
                var stat = await _minIoClient.StatObjectAsync(statArgs);
                return stat != null;
            }
            catch
            {
                return false;
            }
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
