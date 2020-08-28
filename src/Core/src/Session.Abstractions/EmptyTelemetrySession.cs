using System;
using System.Diagnostics.Tracing;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Session.Abstractions
{
    /// <summary>
    /// An <c>Empty</c> telemetry session.
    /// </summary>
    public class EmptyTelemetrySession : ITelemetrySession
    {
        /// <inheritdoc cref="ITelemetrySession"/>
        public void EnableProvider(string name, EventLevel level)
        {
        }

        /// <inheritdoc cref="ITelemetrySession"/>
        public void Attach(ITelemetryEventTransmitter transmitter)
        {
        }

        /// <inheritdoc cref="IDisposable"/>
        public void Dispose()
        {
        }
    }
}