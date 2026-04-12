using backend.Models.Enums;
using System;

namespace backend.Models.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool ProfileCreated { get; set; }
        public UserRole Role { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
}