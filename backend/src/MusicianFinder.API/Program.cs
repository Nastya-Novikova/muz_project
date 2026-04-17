using MusicianFinder.API.Extensions;
using MusicianFinder.API.Middleware;
using MusicianFinder.Application;               // Для метода AddApplication
using MusicianFinder.Infrastructure.Extensions; // Для метода AddInfrastructure
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Регистрация сервисов
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddApplication();                // Регистрация слоя Application
builder.Services.AddInfrastructure(builder.Configuration); // Регистрация слоя Infrastructure

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Применение миграций
app.ApplyMigrations();

app.Run();