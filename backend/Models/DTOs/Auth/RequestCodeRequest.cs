using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Auth
{
    public class RequestCodeRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}