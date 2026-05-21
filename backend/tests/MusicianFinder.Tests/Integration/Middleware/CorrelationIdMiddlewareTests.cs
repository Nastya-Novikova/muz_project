using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using MusicianFinder.API.Middleware;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Middleware
{
    public class CorrelationIdMiddlewareTests : TestBase
    {
        public CorrelationIdMiddlewareTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Invoke_WhenNoHeader_AddsCorrelationId()
        {
            using var host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app =>
                        {
                            app.UseMiddleware<CorrelationIdMiddleware>();
                            app.Run(context => context.Response.WriteAsync("OK"));
                        });
                })
                .Start();

            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Headers.Should().ContainKey("X-Correlation-Id");
            var correlationId = response.Headers.GetValues("X-Correlation-Id").First();
            correlationId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Invoke_WhenHeaderExists_PropagatesSameId()
        {
            using var host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app =>
                        {
                            app.UseMiddleware<CorrelationIdMiddleware>();
                            app.Run(context =>
                            {
                                var requestId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
                                context.Response.Headers["X-Correlation-Id"] = requestId;
                                return context.Response.WriteAsync("OK");
                            });
                        });
                })
                .Start();

            var client = host.GetTestClient();
            var expectedId = "my-custom-id-123";
            client.DefaultRequestHeaders.Add("X-Correlation-Id", expectedId);
            var response = await client.GetAsync("/");
            response.Headers.GetValues("X-Correlation-Id").First().Should().Be(expectedId);
        }
    }
}