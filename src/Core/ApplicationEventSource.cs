using System.Diagnostics.Tracing;

namespace Thor.Core
{
    [EventSource(Name = EventSourceNames.Application)]
    internal sealed class ApplicationEventSource
        : EventSourceBase
    {
        private const int _startEventId = 1;
        private const int _stopEventId = 2;

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

        #region Tasks and Opcodes

        public static class Tasks
        {
            public const EventTask Application = (EventTask)1;
        }

        #endregion
    }
}