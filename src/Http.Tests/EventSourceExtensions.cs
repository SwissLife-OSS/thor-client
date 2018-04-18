using System;
using System.Diagnostics.Tracing;
using Thor.Analyzer;

namespace Thor.Core.Http.Tests
{
    public static class EventSourceExtensions
    {
        public static void ProbeEvents(this EventSource eventSource,
            Action<ProbeEventListener> execute)
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                try
                {
                    listener.EnableEvents(eventSource, EventLevel.Verbose);
                    execute(listener);
                }
                finally
                {
                    listener.DisableEvents(eventSource);
                }
            }
        }
    }
}