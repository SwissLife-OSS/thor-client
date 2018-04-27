using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Represents a generalized event in the pipeline (event stream).
    /// </summary>
    public class TelemetryEvent
    {
        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the attachment id.
        /// </summary>
        public string AttachmentId { get; set; }

        /// <summary>
        /// Gets or sets an id that defines an 'audience' for the event (admin, operational, ...).
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// Gets or sets the environment id.
        /// </summary>
        public int EnvironmentId { get; set; }

        /// <summary>
        /// Gets or sets the provider-specific integer value that uniquely identifies an event
        /// within the scope of the provider.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets provider-specific groups of events which can be enabled and disabled independently.
        /// </summary>
        public EventKeywords Keywords { get; set; }

        /// <summary>
        /// Gets or sets the verbosity of the event (Critical, Error, ..., Info, Verbose)
        /// </summary>
        public EventLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the event message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name for the event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the payloads.
        /// </summary>
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Gets or sets the process id for the process that logged the event.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets a short name for the process.
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the provider id that uniquely identifies the provider for this event.
        /// </summary>
        public Guid ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider associated with the event.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets a numeric id that identifies the particular event within the group of
        /// events identified by the event's task.
        /// </summary>
        public int OpcodeId { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name for the event's opcode.
        /// </summary>
        public string OpcodeName { get; set; }

        /// <summary>
        /// Gets or sets the related activity id (which means the parent id).
        /// </summary>
        public Guid RelatedActivityId { get; set; }

        /// <summary>
        /// Gets or sets the name of the event session.
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// Gets or sets a group id called a Task that indicates the broad area within the provider 
        /// that the event pertains to (for example the Kernel provider has Tasks for Process, Threads, etc).
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name for the event's task (group of related events).
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Gets or sets the thread id for the thread that logged the event.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of an event.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the version number for this event.
        /// </summary>
        public int Version { get; set; }
    }
}