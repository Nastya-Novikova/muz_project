using MediatR;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Запрос для получения списка всех музыкальных специальностей.
    /// </summary>
    public class GetSpecialtiesQuery : IQuery<List<LookupItemDto>>
    {
    }
}