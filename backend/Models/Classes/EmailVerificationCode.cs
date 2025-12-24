using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Classes;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Код подтверждения email
/// </summary>
[Table("EmailVerificationCodes")]
public class EmailVerificationCode
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Email
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 6-значный код
    /// </summary>
    [Required, StringLength(6)]
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