using System;

namespace MusicianFinder.Tests.Shared
{
    /// <summary>
    /// Данные тестового пользователя: токен, идентификаторы и email.
    /// </summary>
    public record TestUserData(string Token, Guid UserId, Guid ProfileId, string Email);
}