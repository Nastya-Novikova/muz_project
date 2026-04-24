using MediatR;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для получения профиля текущего пользователя.
    /// </summary>
    public class GetMyProfileQuery : IQuery<ProfileDto>
    {
    }
}