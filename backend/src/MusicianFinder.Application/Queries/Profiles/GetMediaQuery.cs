using MediatR;
using MusicianFinder.Application.DTOs.Media;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Profiles
{
    /// <summary>
    /// Запрос для получения медиа-контента профиля.
    /// </summary>
    public class GetMediaQuery : IQuery<MediaDto>
    {
        /// <summary>
        /// Идентификатор профиля.
        /// </summary>
        public Guid ProfileId { get; set; }
    }
}