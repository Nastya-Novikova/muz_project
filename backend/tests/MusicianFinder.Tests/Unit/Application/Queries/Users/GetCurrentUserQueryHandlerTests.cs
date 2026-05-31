using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Interfaces;
using MusicianFinder.Application.Interfaces.ReadRepositories;
using MusicianFinder.Application.Queries.Users;
using MusicianFinder.Tests.Shared;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Unit.Application.Queries.Users
{
    public class GetCurrentUserQueryHandlerTests : TestBase
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly GetCurrentUserQueryHandler _handler;

        public GetCurrentUserQueryHandlerTests(ITestOutputHelper output) : base(output)
        {
            _userReadRepository = Substitute.For<IUserReadRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _handler = new GetCurrentUserQueryHandler(_userReadRepository, _currentUserService);
        }

        [Fact]
        public async Task Handle_ReturnsCurrentUser()
        {
            var userId = Guid.NewGuid();
            var userDto = new UserDto { Id = userId, Email = "test@example.com" };
            _currentUserService.UserId.Returns(userId);
            _userReadRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(userDto);

            var query = new GetCurrentUserQuery();
            var result = await _handler.Handle(query, CancellationToken.None);
            result.Should().Be(userDto);
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _currentUserService.UserId.Returns(userId);
            _userReadRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((UserDto?)null);

            var query = new GetCurrentUserQuery();
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}