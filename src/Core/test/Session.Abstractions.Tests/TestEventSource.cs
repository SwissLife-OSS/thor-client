using System.Diagnostics.Tracing;

namespace Thor.Core
{
    [EventSource(Name = "Thor-Test")]
    public sealed class TestEventSource
        : EventSource
    {
        public static TestEventSource Log { get; } = new TestEventSource();

        private TestEventSource() { }

        [Event(1, Level = EventLevel.Informational, Message = "Test message 555", Version = 1)]
        public void DoSomething()
        {
            WriteEvent(1);
        }
    }
}