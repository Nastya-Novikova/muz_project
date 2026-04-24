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
    [Authorize]
    public class MeEventsController : BaseApiController
    {
        /// <summary>
        /// Получить мероприятия, созданные текущим пользователем.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с мероприятиями.</returns>
        [HttpGet("created")]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCreatedEvents([FromQuery] GetMyCreatedEventsQuery query)
        {
            var result = await Mediator.Send(query);
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
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}