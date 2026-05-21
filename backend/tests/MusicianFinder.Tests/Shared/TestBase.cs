using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Shared
{
    /// <summary>
    /// Базовый класс для всех тестов, предоставляющий метод логирования через <see cref="ITestOutputHelper"/>.
    /// </summary>
    public abstract class TestBase
    {
        private readonly ITestOutputHelper _output;

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>Логирует сообщение с временной меткой.</summary>
        protected void Log(string message)
        {
            _output.WriteLine($"[{System.DateTime.Now:HH:mm:ss.fff}] {message}");
        }

        /// <summary>Логирует информационное сообщение.</summary>
        protected void LogInfo(string message) => Log($"[INFO] {message}");

        /// <summary>Логирует предупреждение.</summary>
        protected void LogWarning(string message) => Log($"[WARN] {message}");

        /// <summary>Логирует ошибку.</summary>
        protected void LogError(string message) => Log($"[ERROR] {message}");

        /// <summary>Логирует исключение.</summary>
        protected void LogException(Exception ex) => Log($"[EXCEPTION] {ex.Message}\n{ex.StackTrace}");
    }
}