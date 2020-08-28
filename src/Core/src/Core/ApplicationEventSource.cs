using System;
using System.Diagnostics.Tracing;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core
{
    [EventSource(Name = EventSourceNames.Application)]
    internal sealed class ApplicationEventSource
        : EventSourceBase
    {
        private const int _startEventId = 1;
        private const int _stopEventId = 2;
        private const int _unhandledEventId = 3;

        public static ApplicationEventSource Log { get; } = new ApplicationEventSource();

        private ApplicationEventSource() { }

        [Event(_startEventId, Level = EventLevel.LogAlways, Message = "Starting application with id '{0}'...",
            Task = Tasks.Application, Opcode = EventOpcode.Start)]
        public void Start(int applicationId) =>
            WriteEvent(_startEventId, applicationId);

        [Event(_stopEventId, Level = EventLevel.LogAlways, Message = "Stopping application with id '{0}'...",
            Task = Tasks.Application, Opcode = EventOpcode.Stop)]
        public void Stop(int applicationId) =>
            WriteEvent(_stopEventId, applicationId);
        
        [NonEvent]
        public void UnhandledExceptionOccurred(Exception exception)
        {
            if (IsEnabled())
            {
                AttachmentId attachmentId = AttachmentId.NewId();
                ExceptionAttachment attachment = AttachmentFactory.Create(attachmentId,
                    nameof(exception), exception);

                AttachmentDispatcher.Instance.Dispatch(attachment);
                UnhandledExceptionOccurred(Application.Id, ActivityStack.Id, attachmentId);
            }
        }

        [Event(_unhandledEventId, Level = EventLevel.Error,
            Message = "Unhandled exception occurred.", Version = 1)]
        private void UnhandledExceptionOccurred(
            int applicationId, Guid activityId, string attachmentId)
        {
            UnhandledExceptionOccurredCore(
                _unhandledEventId, applicationId, activityId, attachmentId);
        }

        [NonEvent]
        private unsafe void UnhandledExceptionOccurredCore(
            int eventId, int applicationId, Guid activityId, string attachmentId)
        {
            StringExtensions.SetToEmptyIfNull(ref attachmentId);

            fixed (char* attachmentIdBytes = attachmentId)
            {
                const short dataCount = 3;
                EventData* data = stackalloc EventData[dataCount];

                data[0].DataPointer = (IntPtr)(&applicationId);
                data[0].Size = 4;
                data[1].DataPointer = (IntPtr)(&activityId);
                data[1].Size = 16;
                data[2].DataPointer = (IntPtr)attachmentIdBytes;
                data[2].Size = ((attachmentId.Length + 1) * 2);

                WriteEventCore(eventId, dataCount, data);
            }
        }

        #region Tasks and Opcodes

        public static class Tasks
        {
            public const EventTask Application = (EventTask)1;
        }

        #endregion
    }
}