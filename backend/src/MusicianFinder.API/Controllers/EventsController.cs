using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Events.CancelEvent;
using MusicianFinder.Application.Features.Events.CreateEvent;
using MusicianFinder.Application.Features.Events.GetEventById;
using MusicianFinder.Application.Features.Events.GetEvents;
using MusicianFinder.Application.Features.Events.GetMyCreatedEvents;
using MusicianFinder.Application.Features.Events.GetMyRegisteredEvents;
using MusicianFinder.Application.Features.Events.RegisterToEvent;
using MusicianFinder.Application.Features.Events.UnregisterFromEvent;
using MusicianFinder.Application.Features.Events.UpdateEvent;
using MusicianFinder.Application.Features.Events.UploadEventImage;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер мероприятий.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EventsController"/>.
        /// </summary>
        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить список мероприятий.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] GetEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить мероприятие по ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEventByIdQuery { EventId = id });
            return Ok(result);
        }

        /// <summary>
        /// Создать мероприятие.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEventCommand command)
        {
            var eventId = await _mediator.Send(command);
            return Ok(new { id = eventId });
        }

        /// <summary>
        /// Обновить мероприятие.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventCommand command)
        {
            command.EventId = id;
            await _mediator.Send(command);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Отменить мероприятие.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _mediator.Send(new CancelEventCommand { EventId = id });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Зарегистрироваться на мероприятие.
        /// </summary>
        [HttpPost("{id}/register")]
        [Authorize]
        public async Task<IActionResult> Register(Guid id)
        {
            await _mediator.Send(new RegisterToEventCommand { EventId = id });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Отменить регистрацию.
        /// </summary>
        [HttpDelete("{id}/register")]
        [Authorize]
        public async Task<IActionResult> Unregister(Guid id)
        {
            await _mediator.Send(new UnregisterFromEventCommand { EventId = id });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Загрузить изображение мероприятия.
        /// </summary>
        [HttpPost("{id}/image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile image)
        {
            var command = new UploadEventImageCommand
            {
                EventId = id,
                FileStream = image.OpenReadStream(),
                FileName = image.FileName,
                ContentType = image.ContentType
            };
            var url = await _mediator.Send(command);
            return Ok(new { imageUrl = url });
        }

        /// <summary>
        /// Получить созданные мной мероприятия.
        /// </summary>
        [HttpGet("my/created")]
        [Authorize]
        public async Task<IActionResult> GetMyCreated([FromQuery] GetMyCreatedEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить мероприятия, на которые я записан.
        /// </summary>
        [HttpGet("my/registered")]
        [Authorize]
        public async Task<IActionResult> GetMyRegistered([FromQuery] GetMyRegisteredEventsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}