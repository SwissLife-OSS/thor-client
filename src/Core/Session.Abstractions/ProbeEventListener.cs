using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

#if NET461
using System;
using System.Collections.ObjectModel;
#endif

namespace Thor.Core.Session.Abstractions
{
    /// <summary>
    /// An event listener to probe events.
    /// </summary>
    /// <remarks>
    /// Especially good for unit tests. Should not be used in production code.
    /// </remarks>
    public class ProbeEventListener
        : EventListener
    {
        private readonly ConcurrentQueue<EventWrittenEventArgs> _queue = new ConcurrentQueue<EventWrittenEventArgs>();

        /// <summary>
        /// A collection of ordered events which has been recorded during a session.
        /// </summary>
        public IEnumerable<EventWrittenEventArgs> OrderedEvents => _queue.ToArray();

        /// <inheritdoc/>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventId > 0)
            {
                _queue.Enqueue(eventData);
            }
        }

        #if NET461

        /// <summary>
        /// An event whhich is fired when an eventsource is successfully added to the listeners.
        /// </summary>
        public event EventHandler<EventSourceCreatedEventArgs> EventSourceCreated;

        /// <inheritdoc/>
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            EventSourceCreated?.Invoke(this, new EventSourceCreatedEventArgs(eventSource));
        }

        #endif
    }

    #if NET461

    /// <summary>
    /// Provides data for the System.Diagnostics.Tracing.EventListener.EventSourceCreated event.
    /// </summary>
    public class EventSourceCreatedEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceCreatedEventArgs"/> class.
        /// </summary>
        public EventSourceCreatedEventArgs(EventSource eventSource)
        {
            if (eventSource == null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }

            EventSource = eventSource;
        }

        /// <summary>
        /// Get the event source that is attaching to the listener.
        /// </summary>
        public EventSource EventSource { get; }
    }

    #endif
}