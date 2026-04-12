namespace backend.Models.Classes;

/// <summary>
/// Код подтверждения email
/// </summary>
public class EmailVerificationCode
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 6-значный код
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Время создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Использован ли код
    /// </summary>
    public bool IsUsed { get; set; } = false;
}