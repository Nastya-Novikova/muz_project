using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MusicianFinder.Application.Features.Profiles.DTOs;

namespace MusicianFinder.Application.Features.Profiles.GetMyProfile
{
    /// <summary>
    /// Запрос для получения профиля текущего пользователя.
    /// </summary>
    public class GetMyProfileQuery : IRequest<ProfileDto>
    {
    }
}