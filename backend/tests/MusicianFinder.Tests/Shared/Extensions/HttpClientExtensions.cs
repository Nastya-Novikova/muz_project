using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicianFinder.Tests.Shared.Extensions
{
    /// <summary>
    /// Расширения для <see cref="HttpClient"/> для удобной отправки PATCH-запросов.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>Отправляет PATCH-запрос с JSON-телом.</summary>
        public static async Task<HttpResponseMessage> PatchJsonAsync<T>(
            this HttpClient client,
            string requestUri,
            T value,
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri) { Content = content };
            return await client.SendAsync(request, cancellationToken);
        }
    }
}