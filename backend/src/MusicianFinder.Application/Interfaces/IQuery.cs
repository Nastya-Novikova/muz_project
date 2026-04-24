using MediatR;

namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Маркерный интерфейс для запросов (операций чтения).
    /// </summary>
    /// <typeparam name="TResponse">Тип возвращаемого результата.</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}