using MediatR;
using MusicianFinder.Application.DTOs.Profiles;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для получения профиля по идентификатору.
    /// </summary>
    public class GetProfileByIdQuery : IQuery<ProfileDto>
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}