using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Thor.Core;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.HotChocolate
{
    [EventSource(Name = EventSourceNames.HotChocolate)]
    internal sealed class HotChocolateActivityEventSource
        : EventSourceBase
    {
        private const int StartEventId = 1;
        private const int StopEventId = 2;
        private const int BeginTransferEventId = 3;
        private const int EndTransferEventId = 4;
        private const int ValidationErrorEventId = 5;
        private const int QueryErrorEventId = 6;

        public static HotChocolateActivityEventSource Log { get; }
            = new HotChocolateActivityEventSource();

        private HotChocolateActivityEventSource() { }

        /// <summary>
        /// Starts executing query on the server-side.
        /// </summary>
        [NonEvent]
        public void Start(Guid activityId, HotChocolateRequest request)
        {
            AttachmentId attachmentId = AttachmentId.NewId();
            IAttachment attachment = AttachmentFactory
                .Create<HotChocolateRequestAttachment, HotChocolateRequest>(
                    attachmentId, nameof(request), request);

            AttachmentDispatcher.Instance.Dispatch(attachment);
            Start(Application.Id, activityId, attachmentId);
        }

        [Event(StartEventId, Level = EventLevel.LogAlways, Message = "Query start",
            Task = Tasks.Server, Opcode = EventOpcode.Start, Version = 1)]
        private void Start(int applicationId, Guid activityId, string attachmentId)
        {
            WriteEventWithAttachment(
                StartEventId, applicationId, activityId, attachmentId);
        }

        /// <summary>
        /// Stops executing query on the server-side.
        /// </summary>
        [NonEvent]
        public void Stop(Guid activityId)
        {
            Stop(Application.Id, activityId);
        }

        [Event(StopEventId, Level = EventLevel.LogAlways, Message = "Query stop",
            Task = Tasks.Server, Opcode = EventOpcode.Stop, Version = 1)]
        private void Stop(int applicationId, Guid activityId)
        {
            WriteEmptyCore(StopEventId, applicationId, activityId);
        }

        [NonEvent]
        public void BeginTransfer(Guid relatedActivityId)
        {
            BeginTransfer(Application.Id, relatedActivityId);
        }

        [Event(BeginTransferEventId, Level = EventLevel.LogAlways, Message = "Begin activity transfer",
            Task = Tasks.Transfer, Opcode = EventOpcode.Send, Version = 1)]
        private void BeginTransfer(int applicationId, Guid activityId)
        {
            WriteEmptyCore(BeginTransferEventId, applicationId, activityId);
        }

        [NonEvent]
        public void EndTransfer(Guid activityId, Guid relatedActivityId)
        {
            EndTransfer(relatedActivityId, Application.Id, activityId);
        }

        [Event(EndTransferEventId, Level = EventLevel.LogAlways, Message = "End activity transfer",
            Task = Tasks.Transfer, Opcode = EventOpcode.Receive, Version = 1)]
        private void EndTransfer(Guid relatedActivityId, int applicationId, Guid activityId)
        {
            WriteEmptyWithRelatedActivityIdCore(EndTransferEventId, relatedActivityId,
                applicationId, activityId);
        }

        /// <summary>
        /// A validation error occurred during query execution.
        /// </summary>
        [NonEvent]
        public void OnValidationError(IEnumerable<HotChocolateError> errors)
        {
            AttachmentId attachmentId = AttachmentId.NewId();
            HotChocolateErrorsAttachment attachment = AttachmentFactory
                .Create<HotChocolateErrorsAttachment, IEnumerable<HotChocolateError>>(
                    attachmentId, nameof(errors), errors);

            AttachmentDispatcher.Instance.Dispatch(attachment);
            OnValidationError(Application.Id, ActivityStack.Id, attachmentId);
        }

        [Event(ValidationErrorEventId, Level = EventLevel.Error,
            Message = "Validation Error", Version = 1)]
        private void OnValidationError(int applicationId, Guid activityId,
            string attachmentId)
        {
            WriteEventWithAttachment(
                ValidationErrorEventId, applicationId, activityId, attachmentId);
        }

        /// <summary>
        /// A query error occurred during query execution.
        /// </summary>
        [NonEvent]
        public void OnQueryError(Exception exception)
        {
            AttachmentId attachmentId = AttachmentId.NewId();
            ExceptionAttachment attachment = AttachmentFactory
                .Create(attachmentId, nameof(exception), exception);

            AttachmentDispatcher.Instance.Dispatch(attachment);
            OnQueryError(Application.Id, ActivityStack.Id, attachmentId);
        }

        [Event(QueryErrorEventId, Level = EventLevel.Error,
            Message = "Query Error", Version = 1)]
        private void OnQueryError(int applicationId, Guid activityId,
            string attachmentId)
        {
            WriteEventWithAttachment(
                QueryErrorEventId, applicationId, activityId, attachmentId);
        }

        public static class Tasks
        {
            public const EventTask Client = (EventTask)1;
            public const EventTask Server = (EventTask)2;
            public const EventTask Transfer = (EventTask)3;
        }

        [NonEvent]
        private unsafe void WriteEventWithAttachment(
            int eventId, int applicationId, Guid activityId, string attachmentId)
        {
            if (IsEnabled())
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
                    data[4].DataPointer = (IntPtr)attachmentIdBytes;
                    data[4].Size = ((attachmentId.Length + 1) * 2);

                    WriteEventCore(eventId, dataCount, data);
                }
            }
        }
    }
}