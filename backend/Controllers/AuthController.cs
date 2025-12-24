using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend.Services.Interfaces;

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
    public async Task<IActionResult> RequestCode([FromBody] JsonDocument jsonDocument)
    {
        var result = await _service.RequestCodeAsync(jsonDocument);
        return result != null ? Ok(result) : BadRequest();
    }

    /// <summary>
    /// Вход/регистрация по коду
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] JsonDocument jsonDocument)
    {
        var result = await _service.LoginAsync(jsonDocument);
        return result != null ? Ok(result) : BadRequest();
    }
}

/*[HttpPost("request-code")]
public async Task<IActionResult> RequestCode([FromBody] JsonDocument jsonDocument)
{
    var result = await _service.RequestCode(jsonDocument);
    return result != null ? Ok(result) : BadRequest();
}

[HttpPost("verify-code")]
public async Task<IActionResult> VerifyCode([FromBody] JsonDocument jsonDocument)
{
    var result = await _service.VerifyCode(jsonDocument);
    return result != null ? Ok(result) : BadRequest();
}*/