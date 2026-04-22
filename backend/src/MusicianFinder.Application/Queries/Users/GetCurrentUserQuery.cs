using MediatR;
using MusicianFinder.Application.DTOs.Auth;

namespace MusicianFinder.Application.Queries.Users
{
    /// <summary>
    /// Запрос для получения информации о текущем пользователе.
    /// </summary>
    public class GetCurrentUserQuery : IRequest<UserDto>
    {
    }
}