using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.DTOs.Metadata;
using MusicianFinder.Application.Queries.Metadata;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Контроллер справочных данных.
    /// </summary>
    public class ReferenceDataController : BaseApiController
    {
        /// <summary>
        /// Получить список городов.
        /// </summary>
        /// <returns>Список городов.</returns>
        [HttpGet("cities")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCities()
        {
            var result = await Mediator.Send(new GetCitiesQuery());
            return Ok(result);
        }

        /// <summary>
        /// Получить список регионов.
        /// </summary>
        /// <returns>Список регионов.</returns>
        [HttpGet("regions")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegions()
        {
            var result = await Mediator.Send(new GetRegionsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Получить список жанров.
        /// </summary>
        /// <returns>Список жанров.</returns>
        [HttpGet("genres")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenres()
        {
            var result = await Mediator.Send(new GetGenresQuery());
            return Ok(result);
        }

        /// <summary>
        /// Получить список музыкальных специальностей.
        /// </summary>
        /// <returns>Список специальностей.</returns>
        [HttpGet("specialties")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecialties()
        {
            var result = await Mediator.Send(new GetSpecialtiesQuery());
            return Ok(result);
        }

        /// <summary>
        /// Получить список целей сотрудничества.
        /// </summary>
        /// <returns>Список целей.</returns>
        [HttpGet("goals")]
        [ProducesResponseType(typeof(List<LookupItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCollaborationGoals()
        {
            var result = await Mediator.Send(new GetCollaborationGoalsQuery());
            return Ok(result);
        }
    }
}