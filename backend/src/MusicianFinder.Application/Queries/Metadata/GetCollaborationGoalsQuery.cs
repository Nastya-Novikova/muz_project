using MediatR;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Interfaces;

namespace MusicianFinder.Application.Queries.Metadata
{
    /// <summary>
    /// Запрос для получения списка всех целей сотрудничества.
    /// </summary>
    public class GetCollaborationGoalsQuery : IQuery<List<LookupItemDto>>
    {
    }
}