using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MusicianFinder.Domain.Interfaces;
using MusicianFinder.Application.Features.Metadata.DTOs;

namespace MusicianFinder.Application.Features.Metadata.GetCollaborationGoals
{
    /// <summary>
    /// Обработчик запроса <see cref="GetCollaborationGoalsQuery"/>.
    /// </summary>
    public class GetCollaborationGoalsQueryHandler : IRequestHandler<GetCollaborationGoalsQuery, List<LookupItemDto>>
    {
        private readonly ICollaborationGoalRepository _goalRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GetCollaborationGoalsQueryHandler"/>.
        /// </summary>
        /// <param name="goalRepository">Репозиторий целей сотрудничества.</param>
        /// <param name="mapper">Маппер.</param>
        public GetCollaborationGoalsQueryHandler(ICollaborationGoalRepository goalRepository, IMapper mapper)
        {
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<LookupItemDto>> Handle(GetCollaborationGoalsQuery request, CancellationToken cancellationToken)
        {
            var goals = await _goalRepository.GetAllAsync(request.Query, request.SortBy, request.SortDesc);
            return _mapper.Map<List<LookupItemDto>>(goals);
        }
    }
}