using System;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Tests.Shared.Mocks
{
    /// <summary>
    /// Заглушка <see cref="ICurrentUserService"/> для юнит-тестов.
    /// </summary>
    public class MockCurrentUserService : ICurrentUserService
    {
        private readonly Guid _userId;
        private readonly string _email;
        private readonly string _role;
        private readonly bool _isAuthenticated;

        public MockCurrentUserService(Guid userId, string email, string role = "User", bool isAuthenticated = true)
        {
            _userId = userId;
            _email = email;
            _role = role;
            _isAuthenticated = isAuthenticated;
        }

        public Guid UserId => _userId;
        public string Email => _email;
        public string Role => _role;
        public bool IsAuthenticated => _isAuthenticated;
    }
}