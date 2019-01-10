using System.Diagnostics.Tracing;

namespace Thor.Core.Session.Abstractions.Tests
{
    [EventSource(Name = "ProbeEventListener")]
    public sealed class ProbeEventListenerEventSource
       : EventSource
    {
        public static readonly ProbeEventListenerEventSource Log =
            new ProbeEventListenerEventSource();

        [Event(1)]
        public void Foo(string foo)
        {
            WriteEvent(1, foo);
        }

        [Event(2)]
        public void Bar(string bar)
        {
            WriteEvent(2, bar);
        }
    }
}