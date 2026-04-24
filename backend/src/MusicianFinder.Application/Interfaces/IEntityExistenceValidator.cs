namespace MusicianFinder.Application.Interfaces
{
    /// <summary>
    /// Сервис для проверки существования набора справочных сущностей по их идентификаторам.
    /// </summary>
    public interface IEntityExistenceValidator
    {
        /// <summary>
        /// Загружает сущности указанного типа и проверяет, что все запрошенные идентификаторы существуют.
        /// </summary>
        /// <typeparam name="T">Тип справочной сущности (должен иметь свойство Id).</typeparam>
        /// <param name="requestedIds">Список запрошенных идентификаторов (может быть null или пустым).</param>
        /// <param name="entityName">Название сущности для сообщений об ошибке.</param>
        /// <returns>Список загруженных сущностей. Если requestedIds пуст – пустой список.</returns>
        Task<List<T>> LoadAndValidateAsync<T>(List<int>? requestedIds, string entityName)
            where T : class;
    }
}