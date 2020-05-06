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
        public void RetrieveObjectInfo(int id, RequestInformation info)
        {
            if (IsEnabled())
            {
                var attachmentId = AttachmentId.NewId();

                RetrieveObjectInfo(Application.Id, ActivityStack.Id, attachmentId, id);

                AttachmentDispatcher.Instance.Dispatch(
                    AttachmentFactory.Create(attachmentId, "info", info)
                );
            }
        }

        [Event(2, Level = EventLevel.Informational, Message = "Retrieve value for object {3} with information", Version = 1)]
        private void RetrieveObjectInfo(int applicationId, Guid activityId, string attachmentId, int id)
        {
            WriteCore(2, applicationId, activityId, attachmentId, id);
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

        [NonEvent]
        private unsafe void WriteCore(int eventId, int applicationId, Guid activityId, string a, int b)
        {
            if (IsEnabled())
            {
                StringExtensions.SetToEmptyIfNull(ref a);

                fixed (char* aBytes = a)
                {
                    const short dataCount = 4;
                    EventData* data = stackalloc EventData[dataCount];
                    data[0].DataPointer = (IntPtr)(&applicationId);
                    data[0].Size = 4;
                    data[1].DataPointer = (IntPtr)(&activityId);
                    data[1].Size = 16;
                    data[2].DataPointer = (IntPtr)(aBytes);
                    data[2].Size = ((a.Length + 1) * 2);
                    data[3].DataPointer = (IntPtr)(&b);
                    data[3].Size = 4;

                    WriteEventCore(eventId, dataCount, data);
                }
            }
        }
    }
}
