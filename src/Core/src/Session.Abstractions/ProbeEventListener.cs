using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

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
    }
}