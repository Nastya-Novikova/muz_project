namespace MusicianFinder.SharedKernel
{
    /// <summary>
    /// Маркерный интерфейс для инфраструктурных сущностей,
    /// изменения которых не должны расцениваться как мутации агрегатов
    /// при диспатче доменных событий.
    /// </summary>
    public interface IInfrastructureEntity
    {
    }
}