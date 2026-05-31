using System;
using MusicianFinder.Domain.Entities;
using MusicianFinder.Domain.Enums;

namespace MusicianFinder.Tests.Shared.Builders
{
    /// <summary>
    /// Строитель для создания тестовых экземпляров <see cref="User"/>.
    /// </summary>
    public class UserBuilder
    {
        private string _email = "test@example.com";
        private UserRole _role = UserRole.User;
        private bool _profileCreated = false;

        public UserBuilder WithEmail(string email) { _email = email; return this; }
        public UserBuilder WithRole(UserRole role) { _role = role; return this; }
        public UserBuilder WithProfileCreated(bool profileCreated) { _profileCreated = profileCreated; return this; }

        /// <summary>Создаёт экземпляр <see cref="User"/>.</summary>
        public User Build()
        {
            var user = new User(_email);
            if (_profileCreated)
                user.MarkProfileAsCreated();
            return user;
        }
    }
}