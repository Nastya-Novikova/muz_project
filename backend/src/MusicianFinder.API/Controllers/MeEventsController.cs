using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Queries.Events;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с мероприятиями текущего пользователя.
    /// </summary>
    [ApiController]
    [Route("api/me/events")]
    [Authorize]
    public class MeEventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MeEventsController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public MeEventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить мероприятия, созданные текущим пользователем.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с мероприятиями.</returns>
        [HttpGet("created")]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCreatedEvents([FromQuery] GetMyCreatedEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить мероприятия, на которые записан текущий пользователь.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с мероприятиями.</returns>
        [HttpGet("registered")]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyRegisteredEvents([FromQuery] GetMyRegisteredEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}