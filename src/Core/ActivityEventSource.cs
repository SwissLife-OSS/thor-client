using System;
using System.Diagnostics.Tracing;
using Thor.Core.Abstractions;

namespace Thor.Core
{
    [EventSource(Name = EventSourceNames.Activity)]
    internal sealed class ActivityEventSource
        : EventSourceBase
    {
        private const int _startEventId = 1;
        private const int _stopEventId = 2;
        private const int _beginTransferEventId = 3;
        private const int _endTransferEventId = 4;

        public static ActivityEventSource Log { get; } = new ActivityEventSource();

        private ActivityEventSource() { }

        #region Start/Stop Events

        [NonEvent]
        public void Start(Guid activityId, string name) =>
            Start(Application.Id, activityId, name);

        [Event(_startEventId, Level = EventLevel.LogAlways, Message = "Begin '{2}'",
            Task = Tasks.Activity, Opcode = EventOpcode.Start, Version = 4)]
        private unsafe void Start(int applicationId, Guid activityId, string name)
        {
            if (IsEnabled())
            {
                StringExtensions.SetToEmptyIfNull(ref name);

                fixed (char* nameBytes = name)
                {
                    const short dataCount = 3;
                    EventData* data = stackalloc EventData[dataCount];

                    data[0].DataPointer = (IntPtr)(&applicationId);
                    data[0].Size = 4;
                    data[1].DataPointer = (IntPtr)(&activityId);
                    data[1].Size = 16;
                    data[2].DataPointer = (IntPtr)nameBytes;
                    data[2].Size = ((name.Length + 1) * 2);

                    WriteEventCore(_startEventId, dataCount, data);
                }
            }
        }

        [NonEvent]
        public void Stop(Guid activityId, string name) =>
            Stop(Application.Id, activityId, name);

        [Event(_stopEventId, Level = EventLevel.LogAlways, Message = "End '{2}'",
            Task = Tasks.Activity, Opcode = EventOpcode.Stop, Version = 3)]
        private unsafe void Stop(int applicationId, Guid activityId, string name)
        {
            if (IsEnabled())
            {
                StringExtensions.SetToEmptyIfNull(ref name);

                fixed (char* nameBytes = name)
                {
                    const short dataCount = 3;
                    EventData* data = stackalloc EventData[dataCount];

                    data[0].DataPointer = (IntPtr)(&applicationId);
                    data[0].Size = 4;
                    data[1].DataPointer = (IntPtr)(&activityId);
                    data[1].Size = 16;
                    data[2].DataPointer = (IntPtr)nameBytes;
                    data[2].Size = ((name.Length + 1) * 2);

                    WriteEventCore(_stopEventId, dataCount, data);
                }
            }
        }

        #endregion

        #region Begin/End Transfer Events

        [NonEvent]
        public void BeginTransfer(Guid relatedActivityId) =>
            BeginTransfer(Application.Id, relatedActivityId);

        [Event(_beginTransferEventId, Level = EventLevel.LogAlways, Message = "Begin activity transfer",
            Task = Tasks.Transfer, Opcode = EventOpcode.Send, Version = 2)]
        private void BeginTransfer(int applicationId, Guid activityId) =>
            WriteEmptyCore(_beginTransferEventId, applicationId, activityId);

        [NonEvent]
        public void EndTransfer(Guid activityId, Guid relatedActivityId) =>
            EndTransfer(relatedActivityId, Application.Id, activityId);

        [Event(_endTransferEventId, Level = EventLevel.LogAlways, Message = "End activity transfer",
            Task = Tasks.Transfer, Opcode = EventOpcode.Receive, Version = 2)]
        private void EndTransfer(Guid relatedActivityId, int applicationId, Guid activityId) =>
            WriteEmptyWithRelatedActivityIdCore(_endTransferEventId, relatedActivityId, applicationId, activityId);

        #endregion

        #region Tasks and Opcodes

        public static class Tasks
        {
            public const EventTask Activity = (EventTask)1;
            public const EventTask Transfer = (EventTask)2;
        }

        #endregion
    }
}