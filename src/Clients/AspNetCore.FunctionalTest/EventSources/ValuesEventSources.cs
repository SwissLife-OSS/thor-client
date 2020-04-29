using System;
using System.Diagnostics.Tracing;
using Thor.Core;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Hosting.AspNetCore.FunctionalTest.EventSources
{

    [EventSource(Name = "AspNetCore-FunctionalTest")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("thor-generator", "1.0.0")]
    public sealed class ValuesEventSources
        : EventSourceBase
        , IValuesEventSources
    {
        private ValuesEventSources() { }

        public static IValuesEventSources Log { get; } = new ValuesEventSources();


        [NonEvent]
        public void RetrieveObject(int id)
        {
            if (IsEnabled())
            {

                RetrieveObject(Application.Id, ActivityStack.Id, id);

            }
        }

        [Event(1, Level = EventLevel.Informational, Message = "Retrieve value for object {2}", Version = 1)]
        private void RetrieveObject(int applicationId, Guid activityId, int id)
        {
            WriteCore(1, applicationId, activityId, id);
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
