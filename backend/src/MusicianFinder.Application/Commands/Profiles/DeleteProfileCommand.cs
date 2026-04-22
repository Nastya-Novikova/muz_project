using MediatR;

namespace MusicianFinder.Application.Commands.Profiles
{
    /// <summary>
    /// Команда для мягкого удаления профиля текущего пользователя.
    /// </summary>
    public class DeleteProfileCommand : IRequest<Unit>
    {
    }
}