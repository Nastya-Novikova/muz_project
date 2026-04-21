using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Features.Metadata.GetCities;
using MusicianFinder.Application.Features.Metadata.GetCollaborationGoals;
using MusicianFinder.Application.Features.Metadata.GetGenres;
using MusicianFinder.Application.Features.Metadata.GetRegions;
using MusicianFinder.Application.Features.Metadata.GetSpecialties;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер справочных данных.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="MetadataController"/>.
        /// </summary>
        public MetadataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить список городов.
        /// </summary>
        [HttpGet("cities")]
        public async Task<IActionResult> GetCities([FromQuery] GetCitiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список регионов.
        /// </summary>
        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions([FromQuery] GetRegionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список жанров.
        /// </summary>
        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres([FromQuery] GetGenresQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список специальностей.
        /// </summary>
        [HttpGet("specialties")]
        public async Task<IActionResult> GetSpecialties([FromQuery] GetSpecialtiesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Получить список целей сотрудничества.
        /// </summary>
        [HttpGet("goals")]
        public async Task<IActionResult> GetCollaborationGoals([FromQuery] GetCollaborationGoalsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}