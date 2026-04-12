using backend.Services.Interfaces;
using Minio.DataModel.Args;
using Minio;

namespace backend.Services
{
    public class MinioFileStorage : IFileStorage
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;
        private readonly string _publicEndpoint;

        public MinioFileStorage(IConfiguration configuration)
        {
            var endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            _bucketName = configuration["Minio:BucketName"] ?? "musician-files";
            _publicEndpoint = configuration["Minio:PublicEndpoint"] ?? endpoint; // для внешнего доступа

            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false) // в разработке false, в продакшене true с сертификатом
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

            await _minioClient.PutObjectAsync(putObjectArgs);

            // Возвращаем публичный URL. Если бакет публичный, файл будет доступен по прямой ссылке
            return $"http://{_publicEndpoint}/{_bucketName}/{objectName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var objectName = ExtractObjectNameFromUrl(fileUrl);
            if (string.IsNullOrEmpty(objectName)) return;

            var removeArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(removeArgs);
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
                var stat = await _minioClient.StatObjectAsync(statArgs);
                return stat != null;
            }
            catch
            {
                return false;
            }
        }

        private string ExtractObjectNameFromUrl(string url)
        {
            // Пример URL: http://localhost:9000/musician-files/guid_filename.mp3
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');
            if (segments.Length >= 2 && segments[1] == _bucketName)
                return string.Join("/", segments.Skip(2));
            return null;
        }
    }
}
