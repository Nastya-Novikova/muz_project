using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using MusicianFinder.API.Middleware;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.SharedKernel;
using MusicianFinder.Tests.Shared;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Integration.Middleware
{
    public class ErrorHandlingMiddlewareTests : TestBase
    {
        public ErrorHandlingMiddlewareTests(ITestOutputHelper output) : base(output) { }

        private async Task<HttpResponseMessage> RunMiddlewareWithExceptionAsync(Exception exceptionToThrow)
        {
            using var host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .Configure(app =>
                        {
                            app.UseMiddleware<ErrorHandlingMiddleware>();
                            app.Run(context => throw exceptionToThrow);
                        });
                })
                .Start();

            var client = host.GetTestClient();
            return await client.GetAsync("/");
        }

        [Fact]
        public async Task Handle_ValidationException_Returns400WithProblemDetails()
        {
            var exception = new ValidationException("Validation error");
            var response = await RunMiddlewareWithExceptionAsync(exception);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Type.Should().Be("/errors/validation");
            problem.Title.Should().Be("Ошибка валидации");
        }

        [Fact]
        public async Task Handle_DomainException_Returns400()
        {
            var exception = new DomainException("Domain error");
            var response = await RunMiddlewareWithExceptionAsync(exception);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Type.Should().Be("/errors/domain");
            problem.Title.Should().Be("Domain error");
        }

        [Fact]
        public async Task Handle_NotFoundException_Returns404()
        {
            var exception = new NotFoundException("Not found");
            var response = await RunMiddlewareWithExceptionAsync(exception);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Type.Should().Be("/errors/not-found");
        }

        [Fact]
        public async Task Handle_ForbiddenException_Returns403()
        {
            var exception = new ForbiddenException("Forbidden");
            var response = await RunMiddlewareWithExceptionAsync(exception);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Type.Should().Be("/errors/forbidden");
        }

        [Fact]
        public async Task Handle_ConflictException_Returns409()
        {
            var exception = new ConflictException("Conflict");
            var response = await RunMiddlewareWithExceptionAsync(exception);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Type.Should().Be("/errors/conflict");
        }

        [Fact]
        public async Task Handle_UnhandledException_Returns500()
        {
            var exception = new InvalidOperationException("Unexpected");
            var response = await RunMiddlewareWithExceptionAsync(exception);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem!.Type.Should().Be("/errors/server");
            problem.Title.Should().Be("Внутренняя ошибка сервера");
        }
    }
}