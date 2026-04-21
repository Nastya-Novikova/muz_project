using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MusicianFinder.Application.Features.Profiles.DeleteProfile
{
    /// <summary>
    /// Команда для мягкого удаления профиля.
    /// </summary>
    public class DeleteProfileCommand : IRequest<Unit>
    {
    }
}