using MediatR;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Маркерный интерфейс для команд (модифицирующих операций).
    /// </summary>
    /// <typeparam name="TResponse">Тип возвращаемого результата.</typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}