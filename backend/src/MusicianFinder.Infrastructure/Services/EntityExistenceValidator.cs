using Microsoft.EntityFrameworkCore;
using MusicianFinder.Application.Core.Exceptions;
using MusicianFinder.Application.Interfaces;
using FluentValidation.Results;
using ValidationException = MusicianFinder.Application.Core.Exceptions.ValidationException;

namespace MusicianFinder.Infrastructure.Services
{
    /// <summary>
    /// Реализация сервиса проверки существования справочных сущностей.
    /// </summary>
    public class EntityExistenceValidator : IEntityExistenceValidator
    {
        private readonly IReadDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="EntityExistenceValidator"/>.
        /// </summary>
        /// <param name="dbContext">Контекст для доступа к данным.</param>
        public EntityExistenceValidator(IReadDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<List<T>> LoadAndValidateAsync<T>(List<int>? requestedIds, string entityName)
            where T : class
        {
            if (requestedIds is null || requestedIds.Count == 0)
                return [];

            IQueryable<T> source = typeof(T).Name switch
            {
                nameof(Domain.Entities.Genre) => (IQueryable<T>)_dbContext.Genres,
                nameof(Domain.Entities.MusicalSpecialty) => (IQueryable<T>)_dbContext.Specialties,
                nameof(Domain.Entities.CollaborationGoal) => (IQueryable<T>)_dbContext.CollaborationGoals,
                _ => throw new InvalidOperationException($"Неподдерживаемый тип сущности: {typeof(T).Name}")
            };

            var entities = await source
                .Where(e => requestedIds.Contains(EF.Property<int>(e, "Id")))
                .ToListAsync();

            if (entities.Count != requestedIds.Count)
            {
                var foundIds = entities.Select(e => EF.Property<int>(e, "Id")).ToList();
                var missingIds = requestedIds.Except(foundIds).ToList();
                throw new ValidationException(new[]
                {
                    new ValidationFailure(entityName,
                        $"Следующие идентификаторы не существуют: {string.Join(", ", missingIds)}.")
                });
            }

            return entities;
        }
    }
}