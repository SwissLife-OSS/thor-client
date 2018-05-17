using System.Collections.Concurrent;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Testing.Utilities
{
    /// <summary>
    /// A transmitter for probing telemetry events.
    /// </summary>
    /// <remarks>
    /// Especially good for unit tests. Should not be used in production code.
    /// </remarks>
    public class ProbeTransmitter
        : ITelemetryTransmitter
    {
        private readonly ConcurrentQueue<TelemetryEvent> _queue = new ConcurrentQueue<TelemetryEvent>();

        /// <inheritdoc/>
        public int Count { get { return _queue.Count; } }

        /// <inheritdoc/>
        public void Enqueue(TelemetryEvent telemetryEvent)
        {
            _queue.Enqueue(telemetryEvent);
        }

        /// <summary>
        /// Dequeues a telemetry event.
        /// </summary>
        /// <returns>A telemetry event if exists; otherwise <c>null</c>.</returns>
        public TelemetryEvent Dequeue()
        {
            if (_queue.TryDequeue(out TelemetryEvent telemetryEvent))
            {
                return telemetryEvent;
            }

            return null;
        }
    }
}