using MediatR;
using MusicianFinder.Application.DTOs.Profiles;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для получения профиля текущего пользователя.
    /// </summary>
    public class GetMyProfileQuery : IRequest<ProfileDto>
    {
    }
}