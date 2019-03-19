using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using HotChocolate;
using Thor.Core;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Extensions.HotChocolate
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
        private const int ResolverErrorEventId = 7;

        public static HotChocolateActivityEventSource Log { get; }
            = new HotChocolateActivityEventSource();

        private HotChocolateActivityEventSource() { }

        /// <summary>
        /// Starts executing query on the server-side.
        /// </summary>
        [NonEvent]
        public void Start(Guid activityId, HotChocolateRequest request)
        {
            if (IsEnabled())
            {
                AttachmentId attachmentId = AttachmentId.NewId();
                IAttachment attachment = AttachmentFactory
                    .Create<HotChocolateRequestAttachment, HotChocolateRequest>(
                        attachmentId, nameof(request), request);

                AttachmentDispatcher.Instance.Dispatch(attachment);
                Start(Application.Id, activityId, attachmentId);
            }
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
            if (IsEnabled())
            {
                Stop(Application.Id, activityId);
            }
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
            if (IsEnabled())
            {
                BeginTransfer(Application.Id, relatedActivityId);
            }
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
            if (IsEnabled())
            {
                EndTransfer(relatedActivityId, Application.Id, activityId);
            }
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
        public void OnValidationError(
            string message,
            HotChocolateRequest request,
            IEnumerable<HotChocolateError> errors)
        {
            if (IsEnabled())
            {
                AttachmentId attachmentId = AttachmentId.NewId();

                IAttachment requestAttachment = AttachmentFactory
                    .Create<HotChocolateRequestAttachment, HotChocolateRequest>(
                        attachmentId, nameof(request), request);

                HotChocolateErrorsAttachment errorAttachments = AttachmentFactory
                    .Create<HotChocolateErrorsAttachment, IEnumerable<HotChocolateError>>(
                        attachmentId, nameof(errors), errors);

                AttachmentDispatcher.Instance.Dispatch(errorAttachments, requestAttachment);

                OnValidationError(Application.Id, ActivityStack.Id,
                    attachmentId, message);
            }
        }

        [Event(ValidationErrorEventId, Level = EventLevel.Error,
            Message = "{3}", Version = 2)]
        private void OnValidationError(int applicationId, Guid activityId,
            string attachmentId, string message)
        {
            WriteEventWithAttachmentAndMessage(
                ValidationErrorEventId, applicationId,
                activityId, attachmentId, message);
        }

        /// <summary>
        /// A query error occurred during query execution.
        /// </summary>
        [NonEvent]
        public void OnQueryError(Exception exception)
        {
            if (IsEnabled())
            {
                AttachmentId attachmentId = AttachmentId.NewId();

                ExceptionAttachment attachment = AttachmentFactory
                    .Create(attachmentId, nameof(exception), exception);

                AttachmentDispatcher.Instance.Dispatch(attachment);

                OnQueryError(
                    Application.Id, ActivityStack.Id,
                    attachmentId, exception.Message);
            }
        }

        [Event(QueryErrorEventId, Level = EventLevel.Error,
            Message = "{3}", Version = 2)]
        private void OnQueryError(int applicationId, Guid activityId,
            string attachmentId, string message)
        {
            WriteEventWithAttachmentAndMessage(
                QueryErrorEventId, applicationId, activityId,
                attachmentId, message);
        }

        /// <summary>
        /// A query error occurred during query execution.
        /// </summary>
        [NonEvent]
        public void OnResolverError(
            string message,
            HotChocolateRequest request,
            IEnumerable<HotChocolateError> errors)
        {
            if (IsEnabled())
            {
                AttachmentId attachmentId = AttachmentId.NewId();

                IAttachment requestAttachment = AttachmentFactory
                    .Create<HotChocolateRequestAttachment, HotChocolateRequest>(
                        attachmentId, nameof(request), request);

                HotChocolateErrorsAttachment errorAttachments = AttachmentFactory
                    .Create<HotChocolateErrorsAttachment, IEnumerable<HotChocolateError>>(
                        attachmentId, nameof(errors), errors);

                AttachmentDispatcher.Instance.Dispatch(
                    errorAttachments, requestAttachment);

                OnResolverError(
                    Application.Id, ActivityStack.Id,
                    attachmentId, message);
            }
        }

        [Event(ResolverErrorEventId, Level = EventLevel.Error,
            Message = "{3}", Version = 2)]
        private void OnResolverError(int applicationId, Guid activityId,
            string attachmentId, string message)
        {
            WriteEventWithAttachmentAndMessage(
                ResolverErrorEventId, applicationId, activityId,
                attachmentId, message);
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

        [NonEvent]
        private unsafe void WriteEventWithAttachmentAndMessage(
            int eventId, int applicationId, Guid activityId,
            string attachmentId, string message)
        {
            StringExtensions.SetToEmptyIfNull(ref attachmentId);
            StringExtensions.SetToEmptyIfNull(ref message);

            fixed (char* messageBytes = message)
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
                data[3].DataPointer = (IntPtr)messageBytes;
                data[3].Size = ((message.Length + 1) * 2);

                WriteEventCore(eventId, dataCount, data);
            }
        }
    }
}