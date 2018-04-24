using System.Diagnostics.Tracing;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// An <c>ETW</c> telemetry session to listen to events.
    /// </summary>
    public interface ITelemetrySession
    {
        /// <summary>
        /// Enables a custom event provider by its name and the desired severity.
        /// </summary>
        /// <param name="name">A provider name.</param>
        /// <param name="level">A level of verbosity.</param>
        void EnableProvider(string name, EventLevel level);

        /// <summary>
        /// Sets a transmitter for telemetry event transmission.
        /// </summary>
        /// <param name="transmitter">A transmitter instance.</param>
        void SetTransmitter(ITelemetryTransmitter transmitter);
    }
}