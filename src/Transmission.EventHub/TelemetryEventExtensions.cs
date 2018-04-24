using System;
using Microsoft.Azure.EventHubs;

namespace Thor.Core.Abstractions
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
        public static EventData Map(this TelemetryEvent source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            byte[] data = source.Serialize();
            EventData destination = new EventData(data);

            return destination;
        }
    }
}