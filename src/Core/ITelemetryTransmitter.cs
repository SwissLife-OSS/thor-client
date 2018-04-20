namespace Thor.Core
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
        void Enqueue(TelemetryEvent telemetryEvent);
    }
}