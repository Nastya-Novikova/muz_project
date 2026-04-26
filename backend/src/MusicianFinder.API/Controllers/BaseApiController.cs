using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicianFinder.Application.Commands.Base;

namespace MusicianFinder.API.Controllers
{
    /// <summary>
    /// Базовый контроллер для конечных точек API.
    /// Предоставляет вспомогательные методы для извлечения заголовка идемпотентности.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediator = null!;

        /// <summary>
        /// Экземпляр <see cref="IMediator"/> для отправки команд и запросов.
        /// </summary>
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        /// <summary>
        /// Считывает заголовок Idempotency-Key из запроса и присваивает его команде, реализующей <see cref="IBaseCommand"/>.
        /// </summary>
        /// <param name="command">Команда, поддерживающая идемпотентность.</param>
        protected void SetIdempotencyKey(IBaseCommand command)
        {
            var key = Request.Headers["Idempotency-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(key))
                command.IdempotencyKey = key;
        }
    }
}