using MediatR;
using MusicianFinder.Application.DTOs.Media;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для получения медиа-контента профиля.
    /// </summary>
    public class GetMediaQuery : IRequest<MediaDto>
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}