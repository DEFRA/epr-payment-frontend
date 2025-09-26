using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.UnitTests.TestHelpers
{
    [ExcludeFromCodeCoverage]
    public class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel, string, Exception?)> LogEntries { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            LogEntries.Add((logLevel, formatter(state, exception), exception));
        }
    }
}
