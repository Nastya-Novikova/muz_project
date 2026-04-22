using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Queries.Metadata;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер справочных данных.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ReferenceDataController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReferenceDataController"/>.
        /// </summary>
        /// <param name="mediator">Экземпляр <see cref="IMediator"/>.</param>
        public ReferenceDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить список городов.
        /// </summary>
        /// <param name="query">Параметры поиска и сортировки.</param>
        /// <returns>Список городов.</returns>
        [HttpGet("cities")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCities([FromQuery] GetCitiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список регионов.
        /// </summary>
        /// <param name="query">Параметры поиска и сортировки.</param>
        /// <returns>Список регионов.</returns>
        [HttpGet("regions")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegions([FromQuery] GetRegionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список жанров.
        /// </summary>
        /// <param name="query">Параметры поиска и сортировки.</param>
        /// <returns>Список жанров.</returns>
        [HttpGet("genres")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenres([FromQuery] GetGenresQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список музыкальных специальностей.
        /// </summary>
        /// <param name="query">Параметры поиска и сортировки.</param>
        /// <returns>Список специальностей.</returns>
        [HttpGet("specialties")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecialties([FromQuery] GetSpecialtiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список целей сотрудничества.
        /// </summary>
        /// <param name="query">Параметры поиска и сортировки.</param>
        /// <returns>Список целей.</returns>
        [HttpGet("goals")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCollaborationGoals([FromQuery] GetCollaborationGoalsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}