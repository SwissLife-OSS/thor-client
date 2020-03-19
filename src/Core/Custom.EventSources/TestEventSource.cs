using System;
using System.Diagnostics.Tracing;
using Thor.Core;
using Thor.Core.Abstractions;

namespace Custom.EventSources
{
    [EventSource(Name = "Custom-Test")]
    public sealed class TestEventSource
        : EventSourceBase
        , ITestEventSource
    {
        private TestEventSource()
        {

        }

        public static ITestEventSource Log { get; } = new TestEventSource();

        [NonEvent]
        public void RunProcess(int processId)
        {
            if (IsEnabled())
            {
                RunProcess(Application.Id, ActivityStack.Id, processId);
            }
        }

        [Event(1, Message = "Run process \"{0}\"", Level = EventLevel.Verbose)]
        private void RunProcess(int applicationId, Guid activityId, int processId)
        {
            WriteCore(1, applicationId, activityId, processId);
        }

        [NonEvent]
        private unsafe void WriteCore(int eventId, int applicationId, Guid activityId, int a)
        {
            if (IsEnabled())
            {
                const short dataCount = 3;
                EventData* data = stackalloc EventData[dataCount];

                data[0].DataPointer = (IntPtr)(&applicationId);
                data[0].Size = 4;
                data[1].DataPointer = (IntPtr)(&activityId);
                data[1].Size = 16;
                data[2].DataPointer = (IntPtr)(&a);
                data[2].Size = 4;

                WriteEventCore(eventId, dataCount, data);
            }
        }
    }
}
