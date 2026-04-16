using backend.Models.DTOs.Common;
using backend.Models.DTOs.Events;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    /// <summary>
    /// Контроллер мероприятий
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Получить ленту мероприятий с фильтрацией и пагинацией
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<EventDto>>> GetEvents([FromQuery] EventFilterRequest filter)
        {
            Guid? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
                currentUserId = GetUserId();

            var result = await _eventService.GetEventsAsync(filter, currentUserId);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>
        /// Получить мероприятие по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(Guid id)
        {
            Guid? currentUserId = null;
            if (User.Identity?.IsAuthenticated == true)
                currentUserId = GetUserId();

            var result = await _eventService.GetByIdAsync(id, currentUserId);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>
        /// Создать мероприятие
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventRequest request)
        {
            var userId = GetUserId();
            var result = await _eventService.CreateAsync(userId, request);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>
        /// Обновить мероприятие (только для создателя)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<EventDto>> Update(Guid id, [FromBody] UpdateEventRequest request)
        {
            var userId = GetUserId();
            var result = await _eventService.UpdateAsync(userId, id, request);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>
        /// Отменить мероприятие (только для создателя)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = GetUserId();
            var result = await _eventService.CancelAsync(userId, id);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Записаться на мероприятие
        /// </summary>
        [HttpPost("{id}/register")]
        [Authorize]
        public async Task<IActionResult> Register(Guid id)
        {
            var userId = GetUserId();
            var result = await _eventService.RegisterAsync(userId, id);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Отменить запись на мероприятие
        /// </summary>
        [HttpDelete("{id}/register")]
        [Authorize]
        public async Task<IActionResult> Unregister(Guid id)
        {
            var userId = GetUserId();
            var result = await _eventService.UnregisterAsync(userId, id);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(new { success = true });
        }

        /// <summary>
        /// Загрузить изображение мероприятия
        /// </summary>
        [HttpPost("{id}/image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile image)
        {
            try
            {
                var userId = GetUserId();
                using var stream = image.OpenReadStream();
                var result = await _eventService.UploadImageAsync(userId, id, stream, image.FileName, image.ContentType);
                if (!result.IsSuccess)
                    return BadRequest(new { error = result.Error });
                return Ok(new { success = true, imageUrl = result.Value });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Получить мероприятия, созданные текущим пользователем
        /// </summary>
        [HttpGet("my/created")]
        [Authorize]
        public async Task<ActionResult<PagedResult<EventDto>>> GetMyCreated(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var userId = GetUserId();
            var result = await _eventService.GetMyCreatedEventsAsync(userId, page, limit);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>
        /// Получить мероприятия, на которые записан текущий пользователь
        /// </summary>
        [HttpGet("my/registered")]
        [Authorize]
        public async Task<ActionResult<PagedResult<EventDto>>> GetMyRegistered(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var userId = GetUserId();
            var result = await _eventService.GetMyRegisteredEventsAsync(userId, page, limit);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        private Guid GetUserId()
        {
            var userIdStr = User.FindFirst("userId")?.Value;
            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }
    }
}
