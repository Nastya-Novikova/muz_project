using MediatR;
using MusicianFinder.Application.DTOs.Profiles;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для получения профиля по идентификатору.
    /// </summary>
    public class GetProfileByIdQuery : IRequest<ProfileDto>
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}