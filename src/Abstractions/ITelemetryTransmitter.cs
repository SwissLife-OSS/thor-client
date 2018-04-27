using System;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A transmitter for telemetry events.
    /// </summary>
    public interface ITelemetryTransmitter
    {
        /// <summary>
        /// Enqueues a single telemetry event for transmission.
        /// </summary>
        /// <param name="telemetryEvent">A telemetry event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="telemetryEvent"/> must not be <c>null</c>.
        /// </exception>
        void Enqueue(TelemetryEvent telemetryEvent);
    }
}