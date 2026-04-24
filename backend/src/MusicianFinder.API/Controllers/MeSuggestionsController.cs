using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Queries.Suggestions;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с предложениями текущего пользователя.
    /// </summary>
    [ApiController]
    [Route("api/me/suggestions")]
    [Authorize]
    public class MeSuggestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MeSuggestionsController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public MeSuggestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить входящие предложения о сотрудничестве.
        /// </summary>
        /// <param name="query">Параметры пагинации и сортировки.</param>
        /// <returns>Страница с предложениями.</returns>
        [HttpGet("received")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReceivedSuggestions([FromQuery] GetReceivedSuggestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить исходящие предложения о сотрудничестве.
        /// </summary>
        /// <param name="query">Параметры пагинации и сортировки.</param>
        /// <returns>Страница с предложениями.</returns>
        [HttpGet("sent")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentSuggestions([FromQuery] GetSentSuggestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}