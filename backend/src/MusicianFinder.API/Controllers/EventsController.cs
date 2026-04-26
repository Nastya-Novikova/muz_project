using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Queries.Events;
using MusicianFinder.API.Contracts.Responses;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Управление мероприятиями.
    /// </summary>
    [ApiController]
    [Route("api/events")]
    public class EventsController : BaseApiController
    {
        /// <summary>
        /// Получить список мероприятий с фильтрацией и пагинацией.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents([FromQuery] GetEventsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Создать новое мероприятие.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CreatedEventResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
        {
            SetIdempotencyKey(command);
            var eventId = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = eventId }, new CreatedEventResponse { Id = eventId });
        }

        /// <summary>
        /// Получить мероприятие по идентификатору.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetEventByIdQuery { EventId = id });
            return Ok(result);
        }

        /// <summary>
        /// Частичное обновление мероприятия.
        /// </summary>
        [HttpPatch("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventCommand command)
        {
            command.EventId = id;
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Отменить мероприятие (мягкое удаление).
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var command = new CancelEventCommand { EventId = id };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Загрузить или обновить изображение мероприятия.
        /// </summary>
        [HttpPut("{id:guid}/image")]
        [Authorize]
        [ProducesResponseType(typeof(FileUploadResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile image)
        {
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            var command = new UploadEventImageCommand
            {
                EventId = id,
                Content = ms.ToArray(),
                FileName = image.FileName,
                ContentType = image.ContentType
            };
            SetIdempotencyKey(command);
            var url = await Mediator.Send(command);
            return Ok(new FileUploadResultDto { Url = url });
        }

        /// <summary>
        /// Зарегистрироваться на мероприятие.
        /// </summary>
        [HttpPost("{id:guid}/registration")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(Guid id)
        {
            var command = new RegisterToEventCommand { EventId = id };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Отменить свою регистрацию на мероприятие.
        /// </summary>
        [HttpDelete("{id:guid}/registration")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unregister(Guid id)
        {
            var command = new UnregisterFromEventCommand { EventId = id };
            SetIdempotencyKey(command);
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Получить мероприятия, созданные текущим пользователем.
        /// </summary>
        [HttpGet("created")]
        [Authorize]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCreatedEvents([FromQuery] GetMyCreatedEventsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить мероприятия, на которые зарегистрирован текущий пользователь.
        /// </summary>
        [HttpGet("registered")]
        [Authorize]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyRegisteredEvents([FromQuery] GetMyRegisteredEventsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}