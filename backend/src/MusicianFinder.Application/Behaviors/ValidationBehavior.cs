using FluentValidation;
using MediatR;
using ValidationException = MusicianFinder.Application.Core.Exceptions.ValidationException;

namespace MusicianFinder.Application.Behaviors
{
    /// <summary>
    /// Поведение MediatR, выполняющее валидацию запроса перед передачей в обработчик.
    /// Если запрос не проходит валидацию, выбрасывается <see cref="ValidationException"/>.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ValidationBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="validators">Коллекция валидаторов для запроса.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}