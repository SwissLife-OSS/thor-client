using System.Collections.Concurrent;
using System.Linq;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Session.Abstractions
{
    /// <summary>
    /// A transmitter for probing telemetry events.
    /// </summary>
    /// <remarks>
    /// Especially good for unit tests. Should not be used in production code.
    /// </remarks>
    public class ProbeTransmitter
        : ITelemetryEventTransmitter
    {
        /// <summary>
        /// Static instance of <see cref="ProbeTransmitter"/>
        /// </summary>
        public static ProbeTransmitter Instance { get; } =
            new ProbeTransmitter();

        private readonly ConcurrentQueue<TelemetryEvent> _queue =
            new ConcurrentQueue<TelemetryEvent>();

        /// <summary>
        /// Number of cached <see cref="TelemetryEvent"/>
        /// </summary>
        public int Count { get { return _queue.Count; } }

        /// <inheritdoc/>
        public void Enqueue(TelemetryEvent data)
        {
            _queue.Enqueue(data);
        }

        /// <summary>
        /// Check if the transmitter has received specific event.
        /// </summary>
        /// <param name="providerName">The provider name</param>
        /// <param name="eventName">The event name</param>
        /// <returns></returns>
        public bool Contains(string providerName, string eventName)
        {
            return _queue.Any(telemetryEvent =>
                telemetryEvent.ProviderName == providerName &&
                telemetryEvent.Name == eventName);
        }

        /// <summary>
        /// Dequeues a telemetry event.
        /// </summary>
        /// <returns>
        /// A telemetry event if exists; otherwise <c>null</c>.
        /// </returns>
        public TelemetryEvent Dequeue()
        {
            if (_queue.TryDequeue(out TelemetryEvent data))
            {
                return data;
            }

            return null;
        }
    }
}