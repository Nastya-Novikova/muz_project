using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MusicianFinder.Tests.Shared.Logging
{
    /// <summary>
    /// Провайдер логгера, направляющий вывод в <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestOutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;
        public TestOutputLoggerProvider(ITestOutputHelper output) => _output = output;
        public ILogger CreateLogger(string categoryName) => new TestOutputLogger(_output, categoryName);
        public void Dispose() { }
    }

    /// <summary>
    /// Реализация <see cref="ILogger"/>, записывающая сообщения в <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class TestOutputLogger : ILogger
    {
        private readonly ITestOutputHelper _output;
        private readonly string _categoryName;

        public TestOutputLogger(ITestOutputHelper output, string categoryName)
        {
            _output = output;
            _categoryName = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            var formattedMessage = $"{DateTime.Now:HH:mm:ss.fff} [{logLevel}] [{_categoryName}] {message}";
            _output.WriteLine(formattedMessage);
        }
    }
}