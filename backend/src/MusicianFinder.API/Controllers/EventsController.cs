using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Events;
using MusicianFinder.Application.Core.Behaviors;
using MusicianFinder.Application.Core.Pagination;
using MusicianFinder.Application.DTOs.Events;
using MusicianFinder.Application.Queries.Events;
using MusicianFinder.API.Contracts.Responses;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер для работы с мероприятиями.
    /// </summary>
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventsController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить список мероприятий с фильтрацией и пагинацией.
        /// </summary>
        /// <param name="query">Параметры фильтрации и пагинации.</param>
        /// <returns>Страница с мероприятиями.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<EventDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents([FromQuery] GetEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Создать новое мероприятие.
        /// </summary>
        /// <param name="command">Данные мероприятия.</param>
        /// <returns>Идентификатор созданного мероприятия.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CreatedEventResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
        {
            SetIdempotencyKey(command);
            var eventId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = eventId }, new CreatedEventResponse { Id = eventId });
        }

        /// <summary>
        /// Получить мероприятие по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <returns>Мероприятие.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEventByIdQuery { EventId = id });
            return Ok(result);
        }

        /// <summary>
        /// Полностью обновить мероприятие.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <param name="command">Данные для обновления.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventCommand command)
        {
            command.EventId = id;
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Отменить мероприятие (мягкое удаление).
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var command = new CancelEventCommand { EventId = id };
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Загрузить или обновить изображение мероприятия.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <param name="image">Файл изображения.</param>
        /// <returns>URL загруженного изображения.</returns>
        [HttpPost("{id:guid}/image")]
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
            var url = await _mediator.Send(command);
            return Ok(new FileUploadResultDto { Url = url });
        }

        /// <summary>
        /// Записаться на мероприятие.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpPost("{id:guid}/registrations")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(Guid id)
        {
            var command = new RegisterToEventCommand { EventId = id };
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Отменить свою регистрацию на мероприятие.
        /// </summary>
        /// <param name="id">Идентификатор мероприятия.</param>
        /// <returns>Статус 204 No Content.</returns>
        [HttpDelete("{id:guid}/registrations/me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unregister(Guid id)
        {
            var command = new UnregisterFromEventCommand { EventId = id };
            SetIdempotencyKey(command);
            await _mediator.Send(command);
            return NoContent();
        }

        private void SetIdempotencyKey(IBaseCommand command)
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                command.IdempotencyKey = key;
        }
    }
}