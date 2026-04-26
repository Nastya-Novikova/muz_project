using MediatR;
using MusicianFinder.Application.DTOs.Auth;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Users
{
    /// <summary>
    /// Запрос для получения информации о текущем пользователе.
    /// </summary>
    public class GetCurrentUserQuery : IQuery<UserDto>
    {
    }
}