using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Collaborations.CheckCollaboration;
using MusicianFinder.Application.Features.Collaborations.GetReceivedSuggestions;
using MusicianFinder.Application.Features.Collaborations.GetSentSuggestions;
using MusicianFinder.Application.Features.Collaborations.SendSuggestion;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер предложений о сотрудничестве.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CollaborationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CollaborationsController"/>.
        /// </summary>
        public CollaborationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Отправить предложение.
        /// </summary>
        [HttpPost("{profileId}")]
        public async Task<IActionResult> SendSuggestion(Guid profileId, [FromBody] SendSuggestionCommand command)
        {
            command.ToProfileId = profileId;
            await _mediator.Send(command);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Получить входящие предложения.
        /// </summary>
        [HttpGet("received")]
        public async Task<IActionResult> GetReceived([FromQuery] GetReceivedSuggestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить исходящие предложения.
        /// </summary>
        [HttpGet("sent")]
        public async Task<IActionResult> GetSent([FromQuery] GetSentSuggestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Проверить, отправлено ли предложение.
        /// </summary>
        [HttpGet("{profileId}/is-collaborated")]
        public async Task<IActionResult> CheckCollaboration(Guid profileId)
        {
            var result = await _mediator.Send(new CheckCollaborationQuery { CollaboratedProfileId = profileId });
            return Ok(new { isCollaborated = result });
        }
    }
}