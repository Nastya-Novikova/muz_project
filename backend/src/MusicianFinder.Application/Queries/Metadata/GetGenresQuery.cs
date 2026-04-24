using MediatR;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Запрос для получения списка всех музыкальных жанров.
    /// </summary>
    public class GetGenresQuery : IQuery<List<LookupItemDto>>
    {
    }
}