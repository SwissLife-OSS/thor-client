using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmitter for telemetry events.
    /// </summary>
    public interface ITelemetryEventTransmitter
        : ITelemetryTransmitter<TelemetryEvent>
    {
    }
}