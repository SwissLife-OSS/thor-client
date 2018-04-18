using System;
using System.Diagnostics.Tracing;

namespace Thor.Core
{
    /// <summary>
    /// A base class for <see cref="EventSource"/>s.
    /// </summary>
    public class EventSourceBase
        : EventSource
    {
        /// <summary>
        /// Creates a new instance of <see cref="EventSourceBase"/> class.
        /// </summary>
        protected EventSourceBase() { }

        /// <summary>
        /// This routine is a specialized and efficient WriteEvent helper for 
        /// events with a specific message.
        /// </summary>
        /// <param name="eventId">A required event id.</param>
        /// <param name="applicationId">A required application id.</param>
        /// <param name="activityId">A required activity id.</param>
        /// <param name="message">A required message.</param>
        [NonEvent]
        protected unsafe void WriteCore(int eventId, int applicationId, Guid activityId, string message)
        {
            if (IsEnabled())
            {
                StringExtensions.SetToEmptyIfNull(ref message);

                fixed (char* messageBytes = message)
                {
                    const short dataCount = 3;
                    EventData* data = stackalloc EventData[dataCount];

                    data[0].DataPointer = (IntPtr)(&applicationId);
                    data[0].Size = 4;
                    data[1].DataPointer = (IntPtr)(&activityId);
                    data[1].Size = 16;
                    data[2].DataPointer = (IntPtr)messageBytes;
                    data[2].Size = ((message.Length + 1) * 2);

                    WriteEventCore(eventId, dataCount, data);
                }
            }
        }

        /// <summary>
        /// This routine is a specialized and efficient WriteEvent helper for 
        /// events without a message.
        /// </summary>
        /// <param name="eventId">A required event id.</param>
        /// <param name="applicationId">A required application id.</param>
        /// <param name="activityId">A required activity id.</param>
        [NonEvent]
        protected unsafe void WriteEmptyCore(int eventId, int applicationId, Guid activityId)
        {
            if (IsEnabled())
            {
                const short dataCount = 2;
                EventData* data = stackalloc EventData[dataCount];

                data[0].DataPointer = (IntPtr)(&applicationId);
                data[0].Size = 4;
                data[1].DataPointer = (IntPtr)(&activityId);
                data[1].Size = 16;

                WriteEventCore(eventId, dataCount, data);
            }
        }

        /// <summary>
        /// This routine is a specialized and efficient WriteEvent helper for 
        /// events without a message.
        /// </summary>
        /// <param name="eventId">A required event id.</param>
        /// <param name="relatedActivityId">A required related activity id.</param>
        /// <param name="applicationId">A required application id.</param>
        /// <param name="activityId">A required activity id.</param>
        [NonEvent]
        protected unsafe void WriteEmptyWithRelatedActivityIdCore(int eventId, Guid relatedActivityId,
            int applicationId, Guid activityId)
        {
            if (IsEnabled())
            {
                const short dataCount = 2;
                EventData* data = stackalloc EventData[dataCount];

                data[0].DataPointer = (IntPtr)(&applicationId);
                data[0].Size = 4;
                data[1].DataPointer = (IntPtr)(&activityId);
                data[1].Size = 16;

                WriteEventWithRelatedActivityIdCore(eventId, &relatedActivityId, dataCount, data);
            }
        }
    }
}