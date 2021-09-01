using System;
using Azure.Messaging.EventHubs;
using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A bunch of convenient extensions for <see cref="TelemetryEvent"/>.
    /// </summary>
    public static class TelemetryEventExtensions
    {
        /// <summary>
        /// Maps a <see cref="TelemetryEvent"/> to a <see cref="EventData"/> object.
        /// </summary>
        /// <param name="source">A telemetry event.</param>
        /// <returns>A <see cref="EventData"/> object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> must not be <c>null</c>.
        /// </exception>
        public static EventData Map(this TelemetryEvent source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            byte[] data = source.Serialize();

            return new EventData(data);
        }
    }
}
