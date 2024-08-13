using System.Diagnostics;

namespace EPR.Payment.Portal.Common.UnitTests.TestHelpers
{
    public class TestTraceListener : TraceListener
    {
        private readonly List<string> _messages = new List<string>();

        public IReadOnlyList<string> Messages => _messages;

        public override void Write(string? message)
        {
            // Capture messages without newline.
            if (!string.IsNullOrEmpty(message))
            {
                _messages.Add(message);
            }
        }

        public override void WriteLine(string? message)
        {
            // Capture messages with newline.
            if (!string.IsNullOrEmpty(message))
            {
                _messages.Add(message);
            }
        }
    }
}
