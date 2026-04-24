using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Suggestions;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с предложениями о сотрудничестве.
    /// </summary>
    [Authorize]
    public class SuggestionsController : BaseApiController
    {
        /// <summary>
        /// Отправить предложение о сотрудничестве.
        /// </summary>
        /// <param name="command">Данные предложения.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SendSuggestion([FromBody] SendSuggestionCommand command)
        {
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Изменить статус предложения (принять или отклонить).
        /// </summary>
        /// <param name="id">Идентификатор предложения.</param>
        /// <param name="command">Новый статус.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPatch("{id:guid}")]
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