using System;
using System.Diagnostics.Tracing;

namespace Thor.Core.Http
{
    [EventSource(Name = EventSourceNames.Request)]
    internal sealed class RequestEventSource
        : EventSourceBase
    {
        private const int _startEventId = 1;
        private const int _stopEventId = 2;
        private const int _sendEventId = 3;
        private const int _receiveEventId = 4;
        private const int _beginTransferEventId = 5;
        private const int _endTransferEventId = 6;
        private const int _outerScopeWarningEventId = 7;

        public static RequestEventSource Log { get; } = new RequestEventSource();

        private RequestEventSource() { }

        #region Send/Receive Events

        /// <summary>
        /// Initiates a client request on the client-side.
        /// </summary>
        [NonEvent]
        public void Send(Guid activityId, string method, string uri)
        {
            if (IsEnabled())
            {
                Send(Application.Id, activityId, method, uri);
            }
        }

        [Event(_sendEventId, Level = EventLevel.LogAlways, Message = "Initiate {2} {3}",
            Task = Tasks.Client, Opcode = EventOpcode.Start, Version = 1)]
        private void Send(int applicationId, Guid activityId, string method, string uri)
        {
            StartCore(_sendEventId, applicationId, activityId, method, uri, null);
        }

        /// <summary>
        /// Receives a response on the client-side.
        /// </summary>
        [NonEvent]
        public void Receive(Guid activityId, Guid userId, int statusCode, string statusText)
        {
            if (IsEnabled())
            {
                Receive(Application.Id, activityId, userId, statusCode, statusText);
            }
        }

        [Event(_receiveEventId, Level = EventLevel.LogAlways, Message = "Receive {3} {4}",
            Task = Tasks.Client, Opcode = EventOpcode.Stop, Version = 1)]
        private void Receive(int applicationId, Guid activityId, Guid userId, int statusCode,
            string statusText)
        {
            StopCore(_receiveEventId, applicationId, activityId, userId, statusCode, statusText, null);
        }

        #endregion

        #region Start/Stop Events

        /// <summary>
        /// Starts processing a request on the server-side.
        /// </summary>
        [NonEvent]
        public void Start(Guid activityId, string method, string uri)
        {
            if (IsEnabled())
            {
                Start(Application.Id, activityId, method, uri, null);
            }
        }

        /// <summary>
        /// Starts processing a request on the server-side.
        /// </summary>
        [NonEvent]
        public void Start(Guid activityId, HttpRequest request)
        {
            if (IsEnabled())
            {
                AttachmentId id = AttachmentId.NewId();

                Start(Application.Id, activityId, request.Method, request.Uri, id);
                AttachmentDispatcher.Instance.Dispatch(id, nameof(request), request);
            }
        }

        [Event(_startEventId, Level = EventLevel.LogAlways, Message = "Request {2} {3}",
            Task = Tasks.Server, Opcode = EventOpcode.Start, Version = 1)]
        private void Start(int applicationId, Guid activityId, string method, string uri,
            string attachmentId)
        {
            StartCore(_startEventId, applicationId, activityId, method, uri, attachmentId);
        }

        /// <summary>
        /// Stops processing a request on the server-side.
        /// </summary>
        [NonEvent]
        public void Stop(Guid activityId, Guid userId, int statusCode)
        {
            if (IsEnabled())
            {
                Stop(Application.Id, activityId, userId, statusCode, statusCode.GetHttpStatusText(),
                    null);
            }
        }

        /// <summary>
        /// Stops processing a request on the server-side.
        /// </summary>
        [NonEvent]
        public void Stop(Guid activityId, HttpResponse response)
        {
            if (IsEnabled())
            {
                AttachmentId id = AttachmentId.NewId();
                int statusCode = response?.StatusCode ?? 0;
                Guid userId = response?.UserId ?? Guid.Empty;

                Stop(Application.Id, activityId, userId, statusCode, statusCode.GetHttpStatusText(),
                    id.ToString());
                AttachmentDispatcher.Instance.Dispatch(id, nameof(response), response);
            }
        }

        [Event(_stopEventId, Level = EventLevel.LogAlways, Message = "Response {3} {4}",
            Task = Tasks.Server, Opcode = EventOpcode.Stop, Version = 1)]
        private void Stop(int applicationId, Guid activityId, Guid userId, int statusCode,
            string statusText, string attachmentId)
        {
            StopCore(_stopEventId, applicationId, activityId, userId, statusCode, statusText,
                attachmentId);
        }

        #endregion

        #region Outer Scope Warning

        /// <summary>
        /// A warning regarding outer scopes which were not allowed when opening a new server-side message scope.
        /// </summary>
        [NonEvent]
        public void OuterScopeNotAllowed(Guid activityId)
        {
            if (IsEnabled())
            {
                OuterScopeNotAllowed(Application.Id, activityId);
            }
        }

        [Event(_outerScopeWarningEventId, Level = EventLevel.Warning,
            Message = "Outer scopes are not allowed when creating a server-side message scope.",
            Version = 1)]
        private void OuterScopeNotAllowed(int applicationId, Guid activityId)
        {
            WriteEmptyCore(_outerScopeWarningEventId, applicationId, activityId);
        }

        #endregion

        #region Begin/End Transfer Events

        [NonEvent]
        public void BeginTransfer(Guid relatedActivityId)
        {
            if (IsEnabled())
            {
                BeginTransfer(Application.Id, relatedActivityId);
            }
        }

        [Event(_beginTransferEventId, Level = EventLevel.LogAlways, Message = "Begin activity transfer",
            Task = Tasks.Transfer, Opcode = EventOpcode.Send, Version = 1)]
        private void BeginTransfer(int applicationId, Guid activityId)
        {
            WriteEmptyCore(_beginTransferEventId, applicationId, activityId);
        }

        [NonEvent]
        public void EndTransfer(Guid activityId, Guid relatedActivityId)
        {
            if (IsEnabled())
            {
                EndTransfer(relatedActivityId, Application.Id, activityId);
            }
        }

        [Event(_endTransferEventId, Level = EventLevel.LogAlways, Message = "End activity transfer",
            Task = Tasks.Transfer, Opcode = EventOpcode.Receive, Version = 1)]
        private void EndTransfer(Guid relatedActivityId, int applicationId, Guid activityId)
        {
            WriteEmptyWithRelatedActivityIdCore(_endTransferEventId, relatedActivityId,
                applicationId, activityId);
        }

        #endregion

        #region Tasks and Opcodes

        public static class Tasks
        {
            public const EventTask Client = (EventTask)1;
            public const EventTask Server = (EventTask)2;
            public const EventTask Transfer = (EventTask)3;
        }

        #endregion

        [NonEvent]
        private unsafe void StartCore(int eventId, int applicationId, Guid activityId,
            string method, string uri, string attachmentId)
        {
            StringExtensions.SetToEmptyIfNull(ref method);
            StringExtensions.SetToEmptyIfNull(ref uri);
            StringExtensions.SetToEmptyIfNull(ref attachmentId);

            fixed (char* methodBytes = method, uriBytes = uri, attachmentIdBytes = attachmentId)
            {
                const short dataCount = 5;
                EventData* data = stackalloc EventData[dataCount];

                data[0].DataPointer = (IntPtr)(&applicationId);
                data[0].Size = 4;
                data[1].DataPointer = (IntPtr)(&activityId);
                data[1].Size = 16;
                data[2].DataPointer = (IntPtr)methodBytes;
                data[2].Size = ((method.Length + 1) * 2);
                data[3].DataPointer = (IntPtr)uriBytes;
                data[3].Size = ((uri.Length + 1) * 2);
                data[4].DataPointer = (IntPtr)attachmentIdBytes;
                data[4].Size = ((attachmentId.Length + 1) * 2);

                WriteEventCore(eventId, dataCount, data);
            }
        }

        [NonEvent]
        private unsafe void StopCore(int eventId, int applicationId, Guid activityId,
            Guid userId, int statusCode, string statusText, string attachmentId)
        {
            StringExtensions.SetToEmptyIfNull(ref statusText);
            StringExtensions.SetToEmptyIfNull(ref attachmentId);

            fixed (char* statusTextBytes = statusText, attachmentIdBytes = attachmentId)
            {
                const short dataCount = 6;
                EventData* data = stackalloc EventData[dataCount];

                data[0].DataPointer = (IntPtr)(&applicationId);
                data[0].Size = 4;
                data[1].DataPointer = (IntPtr)(&activityId);
                data[1].Size = 16;
                data[2].DataPointer = (IntPtr)(&userId);
                data[2].Size = 16;
                data[3].DataPointer = (IntPtr)(&statusCode);
                data[3].Size = 4;
                data[4].DataPointer = (IntPtr)statusTextBytes;
                data[4].Size = ((statusText.Length + 1) * 2);
                data[5].DataPointer = (IntPtr)attachmentIdBytes;
                data[5].Size = ((attachmentId.Length + 1) * 2);

                WriteEventCore(eventId, dataCount, data);
            }
        }
    }
}