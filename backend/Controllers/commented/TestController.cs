/*using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<TestController> _logger;

    public TestController(IEmailService emailService, ILogger<TestController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        try
        {
            await _emailService.SendVerificationCodeAsync("test@example.com", "123456");
            return Ok("Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email test failed");
            return StatusCode(500, $"Email test failed: {ex.Message}");
        }
    }

    [HttpGet("test-logs")]
    public IActionResult TestLogs()
    {
        _logger.LogTrace("This is a TRACE message");
        _logger.LogDebug("This is a DEBUG message");
        _logger.LogInformation("This is an INFO message");
        _logger.LogWarning("This is a WARNING message");
        _logger.LogError("This is an ERROR message");
        _logger.LogCritical("This is a CRITICAL message");

        return Ok(new { message = "Test logs written" });
    }

    [HttpGet("test-real-email")]
    public async Task<IActionResult> TestRealEmail()
    {
        try
        {
            // Тестовое письмо на реальный email
            await _emailService.SendVerificationCodeAsync("chmo228098@gmail.com", "999999");
            return Ok("Test email sent to real address");
        }
        catch (Exception ex)
        {
            // Логируем детальную ошибку
            _logger.LogError(ex, "Real email test failed: {Message}", ex.Message);
            return StatusCode(500, $"Failed: {ex.Message}");
        }
    }
}*/