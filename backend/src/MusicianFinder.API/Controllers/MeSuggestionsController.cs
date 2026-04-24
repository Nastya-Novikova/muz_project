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
    [Authorize]
    public class MeSuggestionsController : BaseApiController
    {
        /// <summary>
        /// Получить входящие предложения о сотрудничестве.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с предложениями.</returns>
        [HttpGet("received")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReceivedSuggestions([FromQuery] GetReceivedSuggestionsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить исходящие предложения о сотрудничестве.
        /// </summary>
        /// <param name="query">Параметры пагинации.</param>
        /// <returns>Страница с предложениями.</returns>
        [HttpGet("sent")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentSuggestions([FromQuery] GetSentSuggestionsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}