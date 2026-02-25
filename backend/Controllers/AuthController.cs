using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend.Services.Interfaces;
using backend.Models.DTOs.Auth;
using backend.Services;

namespace backend.Controllers;

/// <summary>
/// Контроллер аутентификации
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>
    /// Запрос кода подтверждения на email
    /// </summary>
    [HttpPost("request-code")]
    public async Task<IActionResult> RequestCode([FromBody] RequestCodeRequest request)
    {
        var result = await _service.RequestCodeAsync(request.Email);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { success = true });
    }

    /// <summary>
    /// Вход/регистрация по коду
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _service.LoginAsync(request.Email, request.Code);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}