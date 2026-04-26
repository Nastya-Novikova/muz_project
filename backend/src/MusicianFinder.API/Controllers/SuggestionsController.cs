using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Suggestions;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Suggestions;
using MusicianFinder.Application.Queries.Suggestions;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с предложениями о сотрудничестве (отправка, приём, смена статуса).
    /// Все методы требуют авторизации и используют профиль текущего пользователя.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/suggestions")]
    public class SuggestionsController : BaseApiController
    {
        /// <summary>
        /// Получить входящие предложения о сотрудничестве.
        /// </summary>
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
        [HttpGet("sent")]
        [ProducesResponseType(typeof(PagedResult<SuggestionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentSuggestions([FromQuery] GetSentSuggestionsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Отправить предложение о сотрудничестве.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendSuggestion([FromBody] SendSuggestionCommand command)
        {
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Изменить статус предложения (принять или отклонить).
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSuggestionStatusCommand command)
        {
            command.SuggestionId = id;
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }
    }
}