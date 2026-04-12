namespace backend.Models.Classes;

/// <summary>
/// Справочник городов
/// </summary>
public class City
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Английское название города
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Русское название города
    /// </summary>
    public string LocalizedName { get; set; } = string.Empty;
}