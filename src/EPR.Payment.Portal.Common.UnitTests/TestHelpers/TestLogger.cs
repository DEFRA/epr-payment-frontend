using Microsoft.Extensions.Logging;

namespace EPR.Payment.Portal.Common.UnitTests.TestHelpers
{
    public class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel, string, Exception?)> LogEntries { get; } = new List<(LogLevel, string, Exception?)>();

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            LogEntries.Add((logLevel, formatter(state, exception), exception));
        }
    }
}
