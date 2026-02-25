using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Auth
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(6)]
        public string Code { get; set; } = string.Empty;
    }
}