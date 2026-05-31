using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicianFinder.API.Middleware;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Middleware
{
    public class RequestLoggingMiddlewareTests : TestBase
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddlewareTests(ITestOutputHelper output) : base(output)
        {
            _logger = Substitute.For<ILogger<RequestLoggingMiddleware>>();
        }

        [Fact]
        public async Task Invoke_LogsRequestMethodPathAndStatusCode()
        {
            using var host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddLogging();
                            services.AddSingleton(_logger);
                        })
                        .Configure(app =>
                        {
                            app.UseMiddleware<RequestLoggingMiddleware>();
                            app.Run(context => context.Response.WriteAsync("OK"));
                        });
                })
                .Start();

            var client = host.GetTestClient();
            var response = await client.GetAsync("/test/path");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("GET") && o.ToString()!.Contains("/test/path")),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
    }
}