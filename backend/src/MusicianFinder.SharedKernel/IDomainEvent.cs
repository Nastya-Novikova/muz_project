// SharedKernel/IDomainEvent.cs
using MediatR;

namespace MusicianFinder.SharedKernel
{
    /// <summary>
    /// Маркерный интерфейс для доменных событий.
    /// Наследует <see cref="INotification"/> для интеграции с MediatR.
    /// </summary>
    public interface IDomainEvent
    {
    }
}