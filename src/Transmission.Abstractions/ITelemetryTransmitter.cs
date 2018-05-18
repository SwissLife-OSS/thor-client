using System;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmitter for telemetry data.
    /// </summary>
    public interface ITelemetryTransmitter<TData>
        where TData : class
    {
        /// <summary>
        /// Enqueues a single telemetry data object for transmission.
        /// </summary>
        /// <param name="data">A telemetry data object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> must not be <c>null</c>.
        /// </exception>
        void Enqueue(TData data);
    }
}