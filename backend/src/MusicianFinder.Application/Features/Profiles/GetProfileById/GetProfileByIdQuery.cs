using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Features.Profiles.DTOs;

namespace MusicianFinder.Application.Features.Profiles.GetProfileById
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