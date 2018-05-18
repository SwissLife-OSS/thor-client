using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITelemetryEventTransmitter
        : ITelemetryTransmitter<TelemetryEvent>
    {
    }
}